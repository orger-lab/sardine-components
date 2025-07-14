using System.Drawing;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Sardine.Devices.Hamamatsu.Camera.API;
using Sardine.Optics;

namespace Sardine.Devices.Hamamatsu.Camera
{


    public class HamamatsuCamera : IAreaImagingDevice, INotifyPropertyChanged
    {
        private readonly DCam api;

        public string Name { get; } = string.Empty;
        public double Magnification { get; set; } = 1;
        public Size SensorResolution { get; }
        public SizeF PixelSize { get; }
        public PixelType PixelType { get; }
        public HamamatsuCameraMetadata Metadata { get; }
        public HamamatsuError LastInternalError => api.LastError;
        public bool IsConnected => api.IsConnected;
        public bool IsCapturing => Status == CaptureStatus.Busy;
        public CameraPropertyCollection CameraPropertyCollection => api.PropertyCollection;
        public HamamatsuRingBuffer? FrameBuffer
        {
            get => frameBuffer;
            private set
            {
                if (frameBuffer is not null)
                {
                    frameBuffer.Dispose();
                    frameBuffer = null;
                }
                frameBuffer = value;
            }
        }

        public void ReleaseResources()
        {
            FrameBuffer = null;
        }

        public double BufferUsage
        {
            get => (FrameBuffer?.FramesBehind < 0) ? 1 : ((double)(FrameBuffer?.FramesBehind ?? 0) / BufferSize);
        }

        public double BufferSizeMB
        {
            get => bufferSizeMB; set
            {
                bufferSizeMB = value;
                OnPropertyChanged();
            }
        }

        public int BufferSize
        {
            get => (int)Math.Floor(BufferSizeMB/(CameraPropertyCollection.BufferFrameBytes?.Value ?? 0) * 1024 * 1024);
        }


        private CaptureStatus _status = CaptureStatus.Offline;
        public CaptureStatus Status
        {
            get { return _status; }
            private set
            {
                if (value != _status)
                {
                    _status = value;
                    OnStatusChanged?.Invoke(this, new OnStatusChangedEventArgs(Status));
                    OnPropertyChanged();
                }
            }
        }

        public event EventHandler<OnDCamErrorEventArgs>? OnNewError;
        public event EventHandler<OnStatusChangedEventArgs>? OnStatusChanged;
        public event EventHandler<OnNewFrameEventArgs>? OnNewFrame;
        public event EventHandler? OnExposureChanged;

        #region Capture Region Properties

        public void UpdateCaptureRegion()
        {
            if (CameraPropertyCollection.SubarrayMode is null ||
                CameraPropertyCollection.SubarrayHorizontalPosition is null ||
                CameraPropertyCollection.SubarrayHorizontalSize is null ||
                CameraPropertyCollection.SubarrayVerticalPosition is null ||
                CameraPropertyCollection.SubarrayVerticalSize is null ||
                CameraPropertyCollection.Binning is null)
                throw new InvalidOperationException();

            CameraPropertyCollection.SubarrayMode.Value = Mode.Off;
            CameraPropertyCollection.SubarrayHorizontalPosition.Value = (int)HOffset;
            CameraPropertyCollection.SubarrayHorizontalSize.Value = (int)HSize;
            CameraPropertyCollection.SubarrayVerticalPosition.Value = (int)VOffset;
            CameraPropertyCollection.SubarrayVerticalSize.Value = (int)VSize;
            CameraPropertyCollection.SubarrayMode.Value = Mode.On;
            CameraPropertyCollection.Binning.Value = Binning;

            ValidatedBinning = CameraPropertyCollection.Binning.Value;
            ValidatedHOffset = (uint)CameraPropertyCollection.SubarrayHorizontalPosition.Value;
            ValidatedHSize = (uint)CameraPropertyCollection.SubarrayHorizontalSize.Value;
            ValidatedVOffset = (uint)CameraPropertyCollection.SubarrayVerticalPosition.Value;
            ValidatedVSize = (uint)CameraPropertyCollection.SubarrayVerticalSize.Value;

            Binning = ValidatedBinning;
            HOffset = ValidatedHOffset;
            HSize = ValidatedHSize;
            VOffset = ValidatedVOffset;
            VSize = ValidatedVSize;

            OnPropertyChanged(nameof(CaptureRegionValidated));
            OnPropertyChanged(nameof(MaxExpectedFPS));
            UpdateExposure();
        }



        private uint ValidatedHSize { get; set; }
        private uint ValidatedHOffset { get; set; }
        private uint ValidatedVSize { get; set; }
        private uint ValidatedVOffset { get; set; }
        internal SubarraySize ValidatedBinning { get; set; }
        public bool CaptureRegionValidated => (ValidatedHSize == HSize)
                                              && (ValidatedVSize == VSize)
                                              && (ValidatedHOffset == HOffset)
                                              && (ValidatedVOffset == VOffset)
                                              && (ValidatedBinning == Binning);


        private uint HStep { get; }
        private uint VStep { get; }

        private uint _hsize = 2048;
        public uint HSize
        {
            get => _hsize;
            set
            {
                _hsize = Math.Min((uint)SensorResolution.Width - HOffset, Math.Max(HStep, HStep * (value / HStep)));
                OnPropertyChanged();
                OnPropertyChanged(nameof(CaptureRegionValidated));
            }
        }

        private uint _hpos = 0;
        public uint HOffset
        {
            get => _hpos;
            set
            {
                _hpos = Math.Min((uint)SensorResolution.Width - HSize, (HStep * (value / HStep)));
                OnPropertyChanged();
                OnPropertyChanged(nameof(CaptureRegionValidated));
            }
        }

        private uint _vsize = 2048;
        public uint VSize
        {
            get => _vsize;
            set
            {
                _vsize = Math.Min((uint)SensorResolution.Height - VOffset, Math.Max(VStep, VStep * (value / VStep)));
                OnPropertyChanged();
                OnPropertyChanged(nameof(CaptureRegionValidated));
            }
        }

        private uint _vpos = 0;
        public uint VOffset
        {
            get => _vpos;
            set
            {
                _vpos = Math.Min((uint)SensorResolution.Height - VSize, VStep * (value / VStep));
                OnPropertyChanged();
                OnPropertyChanged(nameof(CaptureRegionValidated));
            }
        }

        public double MaxExpectedFPS
        {
            get 
            {
                return 1/(CameraPropertyCollection.TimingMinTriggerInterval?.Value ?? 0);
            }
        }

        private SubarraySize _binning = SubarraySize.x1;
        public SubarraySize Binning
        {
            get => _binning;
            set
            {
                if (value < SubarraySize.x4)
                    _binning = value;
                else
                    _binning = SubarraySize.x4;

                OnPropertyChanged();
                OnPropertyChanged(nameof(CaptureRegionValidated));
                OnPropertyChanged(nameof(MaxExpectedFPS));
            }
        }

        #endregion

        #region Exposure Properties

        public void UpdateExposure()
        {
            if (CameraPropertyCollection.ExposureTime is null ||
                CameraPropertyCollection.InternalFramerate is null)
                throw new InvalidOperationException();

            CameraPropertyCollection.ExposureTime.Value = Exposure;

            ValidatedExposure = CameraPropertyCollection.ExposureTime.Value;
            ValidatedFramerate = CameraPropertyCollection.InternalFramerate.Value;

            _exposure = ValidatedExposure;
            _framerate = ValidatedFramerate;
            OnExposureChanged?.Invoke(this, EventArgs.Empty);
            OnPropertyChanged(nameof(TimingValidated));
            OnPropertyChanged(nameof(Exposure));
            OnPropertyChanged(nameof(Framerate));
            OnPropertyChanged(nameof(MaxExpectedFPS));
        }

        private double _exposure = 1;
        public double Exposure
        {
            get => _exposure;
            set
            {
                if (_exposure != value)
                {
                    _exposure = value;
                    _framerate = 1 / value;
                    OnPropertyChanged(nameof(Exposure));
                    OnPropertyChanged(nameof(Framerate));
                    OnPropertyChanged(nameof(TimingValidated));
                }
            }
        }

        private double _framerate = 1;
        public double Framerate
        {
            get => _framerate;
            set
            {
                if (_framerate != value)
                {
                    _framerate = value;
                    _exposure = 1 / value;
                    OnPropertyChanged(nameof(Framerate));
                    OnPropertyChanged(nameof(Exposure));
                    OnPropertyChanged(nameof(TimingValidated));
                }
            }
        }

        private double ValidatedExposure { get; set; }
        private double ValidatedFramerate { get; set; }
        public bool TimingValidated => ValidatedExposure == Exposure && ValidatedFramerate == Framerate;
        #endregion

        #region Capture Properties
        public CaptureSetting CaptureSetting
        {
            get => api.CaptureSetting;
            set
            {
                api.CaptureSetting = value;
                OnPropertyChanged();
            }
        }
        public ReadoutSpeed ReadoutSpeed
        {
            get => api.ReadoutSpeed;
            set
            {
                api.ReadoutSpeed = value;
                OnPropertyChanged();
            }
        }

        public uint BundleSize 
        {
            get => bundleSize; set
            {
                bundleSize = Math.Max(1,value);
                OnPropertyChanged();
            }
        }



        private OutputTriggerCollection? outputTriggers;
        public OutputTriggerCollection OutputTriggers
        {
            get => outputTriggers ?? throw new Exception();
            private set
            {
                outputTriggers = value;
                outputTriggers.CollectionChanged += (_, _) => OnPropertyChanged(nameof(OutputTriggers));
                OnPropertyChanged();
            }
        }


        #endregion

        OutputTrigger[] GetOutputTriggers()
        {
            OutputTrigger[] list = new OutputTrigger[CameraPropertyCollection.NumberOfOutputTriggerConnector?.Value ?? 0];

            for (int i = 0; i < list.Length; i++)
            {
                list[i] = OutputTrigger.Get(api, i + 1);
            }

            return list;
        }

        public void SetOutputTrigger(OutputTrigger trigger, int index)
        {
            if (index < (CameraPropertyCollection.NumberOfOutputTriggerConnector?.Value ?? 0))
                trigger.Set(api, index + 1);
        }



        private CaptureTrigger? _captureTrigger;
        private HamamatsuRingBuffer? frameBuffer;
        private uint bundleSize = 1;
        private double bufferSizeMB;

        public CaptureTrigger CaptureTrigger
        {
            get
            {
                _captureTrigger ??= GetCaptureTrigger();
                return _captureTrigger;
            }
            set
            {
                if (!(_captureTrigger?.Equals(value) ?? false))
                {
                    SetCaptureTrigger(value);
                    _captureTrigger = GetCaptureTrigger();
                    OnPropertyChanged();
                }
            }
        }

        CaptureTrigger GetCaptureTrigger()
        {
            return CaptureTrigger.Get(api);
        }

        void SetCaptureTrigger(CaptureTrigger captureTrigger)
        {
            captureTrigger.Set(api);
        }


        private readonly System.Timers.Timer _statusFollowingTimer;



        public static HamamatsuCameraMetadata[] CameraMetadata { get; private set; } = Array.Empty<HamamatsuCameraMetadata>();

        public bool IsDeviceReady => Status == CaptureStatus.Busy;

        public static void FetchCameraMetadata(IHamamatsuAPIWrapper apiWrapper)
        {
            DCam.UnInit(apiWrapper);
            //apiWrapper = apiWrapper.GetNew();
            DCam.Init(apiWrapper, out DCam? testCam);
            if (testCam == null)
            {
                CameraMetadata = Array.Empty<HamamatsuCameraMetadata>();
                return;
            }

            HamamatsuCameraMetadata[] metadata = new HamamatsuCameraMetadata[DCam.DeviceCount];

            for (int camera = 0; camera < DCam.DeviceCount; camera++)
            {
                testCam.CameraID = camera;
                if (testCam.Connect())


                {
                    metadata[camera] = new HamamatsuCameraMetadata(api: apiWrapper,
                                                               id: camera,
                                                               bus: testCam.GetString(DCAMStringID.BUS),
                                                               cameraID: testCam.GetString(DCAMStringID.CAMERAID),
                                                               vendor: testCam.GetString(DCAMStringID.VENDOR),
                                                               model: testCam.GetString(DCAMStringID.MODEL),
                                                               cVersion: testCam.GetString(DCAMStringID.CAMERAVERSION),
                                                               dVersion: testCam.GetString(DCAMStringID.DRIVERVERSION),
                                                               mVersion: testCam.GetString(DCAMStringID.MODULEVERSION),
                                                               apiVersion: testCam.GetString(DCAMStringID.DCAMAPIVERSION),
                                                               seriesName: testCam.GetString(DCAMStringID.CAMERA_SERIESNAME));

                    testCam.Close();
                }
            }

            CameraMetadata = metadata;
        }


        public HamamatsuCamera(int cameraID, bool connect = true, string name = "Camera") : this(CameraMetadata.Where((x) => x.CameraID == cameraID).ToList().FirstOrDefault(), connect, name) { }
        public HamamatsuCamera(HamamatsuCameraMetadata camera, bool connect = true, string name = "Camera")
        {
            if (DCam.Init(camera.api ?? throw new NullReferenceException(), out DCam? apiOut))
            {
                if (apiOut == null)
                    throw new NullReferenceException();
                api = apiOut;
            }
            else throw new NullReferenceException();

            api.OnDCamError += (sender, e) => OnNewError?.Invoke(this, e);
            api.CameraID = camera.ID;
            Metadata = camera;
            Name = name;

            _statusFollowingTimer = new System.Timers.Timer(300);
            _statusFollowingTimer.Elapsed += (_, _) =>
            {
                //Debug.WriteLine($"{this.GetHashCode()} {CameraPropertyCollection.SystemAlive?.Value} {api.Status}");
                Status = (CameraPropertyCollection.SystemAlive?.Value ?? SystemAlive.Offline) == SystemAlive.Online ? api.Status : CaptureStatus.Offline;
                OnPropertyChanged(nameof(BufferUsage));
                OnPropertyChanged(nameof(EstimatedFramerate));
                if (Status == CaptureStatus.Offline) { Disconnect(); }
            };




            if (!Connect())
                throw new Exception();


            OutputTriggers = new OutputTriggerCollection(api, GetOutputTriggers());




            PixelSize = new SizeF((float)(CameraPropertyCollection.ImageDetectorPixelHeight?.Value ?? 0) * 0.000001f,
                                  (float)(CameraPropertyCollection.ImageDetectorPixelHeight?.Value ?? 0) * 0.000001f);

            PixelType = CameraPropertyCollection.ImagePixelType?.Value ?? PixelType.None;

            HStep = (uint)(CameraPropertyCollection.SubarrayHorizontalSize?.Step ?? 0);
            VStep = (uint)(CameraPropertyCollection.SubarrayVerticalSize?.Step ?? 0);

            SensorResolution = new Size((int)((CameraPropertyCollection.SubarrayHorizontalSize?.MaxValue + HStep) ?? 0),
                                        (int)((CameraPropertyCollection.SubarrayVerticalSize?.MaxValue + VStep) ?? 0));

            HSize = (uint)SensorResolution.Width;
            VSize = (uint)SensorResolution.Height;

            Exposure = 0.1;

            //SystemAlive isAlive = CameraPropertyCollection.SystemAlive?.Value ?? SystemAlive.Offline;

            UpdateExposure();
            UpdateCaptureRegion();
            CameraPropertyCollection.DefectCorrectMode!.Value = DefectCorrectMode.On;

            if (!connect)
                Disconnect();
        }

        public bool Connect()
        {
            if (IsConnected)
                return true;

            bool result = api.Connect();

            if (result)
                _statusFollowingTimer.Start();

            return result;
        }
        public void Disconnect()
        {
            _statusFollowingTimer.Stop();
            if (!IsConnected)
                return;
            StopCapture();
            api.Close();
        }

        public void StopCapture()
        {
            if (IsCapturing)
            {
                api.Stop();
            }
            api.ReleaseBuffer();
        }

        public bool PrepareCapture()
        {

            if (CameraPropertyCollection.ReadoutSpeed is not null)
                CameraPropertyCollection.ReadoutSpeed.Value = ReadoutSpeed;

            if (!TestMemoryAvailability(BufferSizeMB))
                return false;

            GC.Collect();
            FrameBuffer = new HamamatsuRingBuffer(api, Metadata, BufferSize, PixelType, ValidatedHSize/(uint)Binning, ValidatedVSize/(uint)Binning, BundleSize);
            GC.Collect();
            FrameBuffer.OnNewFrame += ((sender, e) => OnNewFrame?.Invoke(this, e));

            FrameBuffer.AttachBuffer();

            return (Status != CaptureStatus.Error);
        }

        private bool TestMemoryAvailability(double bufferSize)
        {
            return ((double)api.GetAvailableMemory()/1024 >= bufferSize);
        }

        public double EstimatedFramerate
        {
            get
            {
                if (!IsCapturing)
                    return 0;

                return FrameBuffer?.EstimatedFramerate ?? 0;
            }
        }

        public void StartCapture()
        {
            if (!IsCapturing && FrameBuffer is not null)
            {
                api.Start();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
