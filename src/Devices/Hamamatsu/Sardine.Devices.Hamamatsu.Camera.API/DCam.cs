using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;


namespace Sardine.Devices.Hamamatsu.Camera.API
{
    public partial class DCam : IDisposable
    {
        readonly IHamamatsuAPIWrapper api;
        private bool disposedValue;
        public static bool IsInitialized { get; private set; } = false;
        public static int DeviceCount { get; private set; }

        private int _cameraID = -1;
        public int CameraID
        {
            get => _cameraID;
            set
            {
                if (IsConnected || value >= DeviceCount || value < 0)
                    throw new AccessViolationException();
                _cameraID = value;
            }
        }

        private IntPtr _hdcam = IntPtr.Zero;
        public bool IsConnected => _hdcam != IntPtr.Zero;

        public CameraPropertyCollection PropertyCollection { get; private set; }

        public double GetPropertyValue(int property)
        {
            double value = 0;
            LastError = api.GetValue(_hdcam, property, ref value);

            return value;
        }

        public void SetPropertyValue(int property, double value)
        {
            LastError = api.SetValue(_hdcam, property, value);
        }

        public string GetPropertyName(int property)
        {
            if (!IsConnected) return string.Empty;

            string ret = string.Empty;
            HamamatsuError error = api.GetName(_hdcam, property, ref ret);
            LastError = error;
            return error.Failed() ? string.Empty : ret;
        }

        private void GeneratePropertyCollection()
        {
            HamamatsuError error;
            Dictionary<CameraPropertyID, CameraProperty> dict = new();

            int propID = 0;
            string ret;
            DCAMPropertyAttributeStruct dCAMPropertyAttributeStruct;
            while (true)
            {
                ret = "";
                dCAMPropertyAttributeStruct = new DCAMPropertyAttributeStruct(propID);

                error = api.GetNextID(_hdcam, ref propID, 0);
                if (error.Failed())
                {
                    PropertyCollection = new CameraPropertyCollection(dict);
                    return;
                }

                api.GetName(_hdcam, propID, ref ret);
                api.GetAttribute(_hdcam, ref dCAMPropertyAttributeStruct);

                dict.Add((CameraPropertyID)propID, CameraProperty.GenerateProperty(this, (CameraPropertyID)propID, dCAMPropertyAttributeStruct));
            }
        }

        public CaptureStatus Status
        {
            get
            {
                CaptureStatus stat = CaptureStatus.Error;
                if (IsConnected)
                {
                    HamamatsuError error = api.StatusCap(_hdcam, ref stat);
                    LastError = error;
                    if (error.Failed())
                        return CaptureStatus.Error;
                }

                return stat;
            }
        }




        public CaptureSetting CaptureSetting { get; set; } = CaptureSetting.Sequence;
        public ReadoutSpeed ReadoutSpeed { get; set; } = ReadoutSpeed.Fastest;

        public int FrameSize { get; private set; }

        public int NewestFrameIndex { get; private set; } = -1;
        public int FrameCount { get; private set; } = -1;



        private HamamatsuError _lastError = HamamatsuError.None;
        public HamamatsuError LastError
        {
            get => _lastError;
            private set
            {
                _lastError = value;
                if (value != HamamatsuError.None && value != HamamatsuError.Success)
                {
                    MethodBase? mb = new StackFrame(1).GetMethod();
                    string name = "";
                    if (mb is not null)
                        name = mb.Name;

                    OnDCamError?.Invoke(null, new OnDCamErrorEventArgs(value, name));
                }
            }
        }

        public event EventHandler<OnDCamErrorEventArgs>? OnDCamError;
        public string LastErrorMessage => LastError.ToString();


        private DCam(IHamamatsuAPIWrapper apiIn)
        {
            api = apiIn;
            PropertyCollection = new CameraPropertyCollection(new Dictionary<CameraPropertyID, CameraProperty>());
        }
        public static bool Init(IHamamatsuAPIWrapper apiIn, out DCam? dcam)
        {
            if (!IsInitialized || DeviceCount == 0)
            {
                apiIn.PreInitialize();

                DCAMAPIInit param = new();
                HamamatsuError error = apiIn.Init(ref param);
                if (error.Failed())
                {
                    DeviceCount = 0;
                    dcam = null;
                    IsInitialized = false;
                    return IsInitialized;
                }
                DeviceCount = param.DeviceCountID;
                IsInitialized = true;
            }

            dcam = new DCam(apiIn);
            return IsInitialized;
        }
        public static void UnInit(IHamamatsuAPIWrapper api)
        {
            api.UnInit();
            IsInitialized = false;
        }

        public bool AttachBuffer(DCAMBufferAttach attach)
        {
            HamamatsuError error = api.AttachBuf(_hdcam, ref attach);
            LastError = error;
            return !error.Failed();
        }

        public bool ReleaseBuffer()
        {
            if (!IsConnected)
                return false;

            HamamatsuError error = HamamatsuError.None;

            for (int i = 0; i < 5; i++)
            {
                error = api.ReleaseBuf(_hdcam, i);
                LastError = error;
                if (error.Failed())
                    break;
            }

            return !error.Failed();
        }


        public bool Connect()
        {
            if (!IsInitialized) return false;
            if (IsConnected) return false;
            if (CameraID < 0) return false;

            DCAMDevOpen param = new(CameraID);
            HamamatsuError error = api.OpenDev(ref param);
            LastError = error;
            if (error.Failed())
            {
                _hdcam = IntPtr.Zero;
                api.CloseDev(param.HDCam);
                return false;
            }
            else
                _hdcam = param.HDCam;

            if (IsConnected)
                GeneratePropertyCollection();

            if (PropertyCollection.SystemAlive?.Value != SystemAlive.Online)
            {
                Close();
            }


            return IsConnected;
        }

        public bool Close()
        {
            HamamatsuError error = api.CloseDev(_hdcam);
            LastError = error;
            if (!error.Failed())
                _hdcam = IntPtr.Zero;

            return !IsConnected;
        }

        public string GetString(DCAMStringID iString)
        {
            if (!IsConnected) return string.Empty;

            string ret = string.Empty;
            HamamatsuError error = api.GetStringDev(_hdcam, iString, ref ret);
            LastError = error;

            return error.Failed() ? string.Empty : ret;
        }

        public bool Start()
        {
            if (!IsConnected) return false;

            FrameCount = 0;
            NewestFrameIndex = 0;

            HamamatsuError error = api.StartCap(_hdcam, CaptureSetting);
            LastError = error;

            return !error.Failed();
        }

        public bool Stop()
        {
            if (!IsConnected) return false;

            HamamatsuError error = api.StopCap(_hdcam);
            LastError = error;

            return !error.Failed();
        }

        public bool UpdateFrameTransferInfo(out int newestFrameIndex, out int frameCount)
        {
            newestFrameIndex = -1;
            frameCount = 0;

            if (!IsConnected) return false;

            DCAMCaptureTransferInfo param = new();
            HamamatsuError error = api.TransferInfoCap(_hdcam, ref param);
            if (!error.Failed())
            {
                newestFrameIndex = param.NewestFrameIndex;
                frameCount = param.FrameCount;
                return true;
            }

            LastError = error;
            return false;
        }

        public bool FireTrigger()
        {
            if (!IsConnected) return false;

            HamamatsuError error = api.FireTriggerCap(_hdcam, 0);
            LastError = error;

            return !error.Failed();
        }

        public double GetAvailableMemory()
        {
            return api.GetAvailableMemory();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public DCamWaitHandle GetWaitHandle()
        {
            return new DCamWaitHandle(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                ReleaseBuffer();
                disposedValue = true;
            }
        }
        ~DCam() { Dispose(disposing: false); }
    }
}
