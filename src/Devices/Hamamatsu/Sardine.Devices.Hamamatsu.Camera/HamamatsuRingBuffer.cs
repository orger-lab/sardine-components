using Sardine.Devices.Hamamatsu.Camera.API;

using System.Runtime.InteropServices;

namespace Sardine.Devices.Hamamatsu.Camera
{

    public class HamamatsuRingBuffer : IDisposable
    {
        byte[][] FrameBuffer { get; set; }
        int[][] FramestampBuffer { get; set; }
        DCAMTimeStamp[][] TimestampBuffer { get; set; }
        HamamatsuFrameAliveStatusChecker StatusChecker { get; }

        int[] FramestampBundleID { get; set; }

        public int FrameSize { get; }
        public int BufferSize { get; }
        public PixelType PixelType { get; }
        public uint HSize { get; }
        public uint VSize { get; }
        public uint OutputBundleSize { get; }

        readonly IntPtr[] frameAddresses;
        readonly IntPtr[] timestampAddresses;
        readonly IntPtr[] framestampAddresses;
        readonly IntPtr mainFrameAddress;
        readonly IntPtr mainTimestampAddress;
        readonly IntPtr mainFramestampAddress;

        readonly GCHandle mainFrameHandle;
        readonly GCHandle mainTimestampHandle;
        readonly GCHandle mainFramestampHandle;
        readonly GCHandle[] frameHandles;
        readonly GCHandle[] timestampHandles;
        readonly GCHandle[] framestampHandles;

        public event EventHandler<OnNewFrameEventArgs>? OnNewFrame;

        private Queue<BundleCaptureInfo> frameEventsQueue;

        private bool disposedValue;

        readonly DCam api;
        private readonly HamamatsuCameraMetadata metadata;
        int _readCaret = -1;
        private int ReadCaret
        {
            get => _readCaret;
            set
            {
                _readCaret = value % BufferSize;
            }
        }

        int _writeCaret = -1;
        private int WriteCaret
        {
            get => _writeCaret;
            set
            {
                if (_writeCaret < ReadCaret && value >= ReadCaret ||
                value < _writeCaret && _writeCaret < ReadCaret ||
                value < _writeCaret && value > ReadCaret)
                    IsSkipping = true;


                if (_writeCaret < ConsumeCaret && value >= ConsumeCaret ||
                value < _writeCaret && _writeCaret < ConsumeCaret ||
                value < _writeCaret && value > ConsumeCaret)
                    IsSkipping = true;

                _writeCaret = value;
            }
        }
        const int NumFramesForFramerateEstimation = 15;
        int framerateEstimateCaret = 0;
        readonly long[] timestampsForEstimate;
        public double EstimatedFramerate
        {
            get
            {
                if (timestampsForEstimate.Any((x) => x == 0))
                    return 0;

                double cumEstimatedTimestamp = 0;
                for (int i = framerateEstimateCaret+1; i < framerateEstimateCaret + NumFramesForFramerateEstimation; i++)
                {
                    cumEstimatedTimestamp += timestampsForEstimate[i % NumFramesForFramerateEstimation] - timestampsForEstimate[(i - 1) % NumFramesForFramerateEstimation];
                }

                return (NumFramesForFramerateEstimation - 1) / cumEstimatedTimestamp * TimeSpan.TicksPerSecond;
            }
        }

        public int FrameCount { get; private set; } = -1;

        private int ConsumeCaret { get; set; } = -1;
        public int FramesBehind
        {
            get
            {
                if (IsSkipping)
                    return -1;

                if (WriteCaret >= ConsumeCaret)
                    return WriteCaret - ConsumeCaret;

                return WriteCaret + BufferSize - ConsumeCaret;
            }
        }

        private uint BundleCaret
        {
            get => bundleCaret;
            set => bundleCaret = value % OutputBundleSize;
        }

        public bool IsSkipping { get; private set; } = false;
        public bool IsAttached { get; private set; } = false;


        private DateTime? _time0;
        
        private readonly CancellationTokenSource _tokenSource;
        private readonly Thread _frameCollectionThread;

        internal HamamatsuRingBuffer(DCam apiIn, HamamatsuCameraMetadata metadata, int nFrames, PixelType pType, uint hSize, uint vSize, uint outputBundleSize)
        {
            api = apiIn;
            this.metadata = metadata;
            BufferSize = nFrames;
            PixelType = pType;
            HSize = hSize;
            VSize = vSize;
            OutputBundleSize = outputBundleSize;

            captureInfoArray = new FrameCaptureInfo[OutputBundleSize];

            frameEventsQueue = new(BufferSize);

            FrameSize = (int)api.GetPropertyValue((int)CameraPropertyID.BufferFrameBytes);

            FrameBuffer = new byte[BufferSize][];
            TimestampBuffer = new DCAMTimeStamp[BufferSize][];
            FramestampBuffer = new int[BufferSize][];
            StatusChecker = new HamamatsuFrameAliveStatusChecker(this);


            frameAddresses = new IntPtr[BufferSize];
            timestampAddresses = new IntPtr[BufferSize];
            framestampAddresses = new IntPtr[BufferSize];

            frameHandles = new GCHandle[BufferSize];
            timestampHandles = new GCHandle[BufferSize];
            framestampHandles = new GCHandle[BufferSize];

            timestampsForEstimate = new long[NumFramesForFramerateEstimation];
            for (int i = 0; i < NumFramesForFramerateEstimation; i++)
            {
                timestampsForEstimate[i] = 0;
            }


            for (int i = 0; i < nFrames; i++)
            {
                FrameBuffer[i] = new byte[FrameSize];
                TimestampBuffer[i] = [new DCAMTimeStamp()];
                FramestampBuffer[i] = [0];

                frameHandles[i] = GCHandle.Alloc(FrameBuffer[i], GCHandleType.Pinned);
                timestampHandles[i] = GCHandle.Alloc(TimestampBuffer[i], GCHandleType.Pinned);
                framestampHandles[i] = GCHandle.Alloc(FramestampBuffer[i], GCHandleType.Pinned);

                frameAddresses[i] = frameHandles[i].AddrOfPinnedObject();
                timestampAddresses[i] = timestampHandles[i].AddrOfPinnedObject();
                framestampAddresses[i] = framestampHandles[i].AddrOfPinnedObject();
            }

            FramestampBundleID = new int[BufferSize];


            mainFrameHandle = GCHandle.Alloc(frameAddresses, GCHandleType.Pinned);
            mainFrameAddress = mainFrameHandle.AddrOfPinnedObject();

            mainTimestampHandle = GCHandle.Alloc(timestampAddresses, GCHandleType.Pinned);
            mainTimestampAddress = mainTimestampHandle.AddrOfPinnedObject();

            mainFramestampHandle = GCHandle.Alloc(framestampAddresses, GCHandleType.Pinned);
            mainFramestampAddress = mainFramestampHandle.AddrOfPinnedObject();

            _tokenSource = new CancellationTokenSource();
            _frameCollectionThread = new Thread(new ParameterizedThreadStart(FrameCollector));
        }



        public bool AttachBuffer()
        {
            if (IsAttached)
                throw new InvalidOperationException();

            IsAttached = true;
            DCAMBufferAttach attach;

            attach = new DCAMBufferAttach(BufferSize, mainTimestampAddress, AttachKind.Timestamp);
            if (!api.AttachBuffer(attach))
                return false;

            attach = new DCAMBufferAttach(BufferSize, mainFramestampAddress, AttachKind.Framestamp);
            if (!api.AttachBuffer(attach))
                return false;

            attach = new DCAMBufferAttach(BufferSize, mainFrameAddress, AttachKind.Frame);
            if (!api.AttachBuffer(attach))
                return false;

            _frameCollectionThread.Start(_tokenSource.Token);
            return true;
        }

        FrameCaptureInfo[] captureInfoArray;
        private uint bundleCaret;

        private void FrameCollector(object? ct)
        {
            if (ct == null)
                return;

            CancellationToken cToken = (CancellationToken)ct;

            DCAMWait eventMask = DCAMWait.CAPEVENT.FRAMEREADY | DCAMWait.CAPEVENT.STOPPED;
            DCAMWait newEvent = DCAMWait.NONE;

            DCam.DCamWaitHandle waitHandle = api.GetWaitHandle();
            int _oldFrameCount = -1;
            int currentFrameBundle = 0;
            int outputFrameID = 0;
            int nfIBundle = 0;

            while (!cToken.IsCancellationRequested)
            {
                if (waitHandle.Wait(eventMask, ref newEvent))
                {
                    if (newEvent.FrameReady)
                    {
                        if (api.UpdateFrameTransferInfo(out int _newestFrameIndex, out int _frameCount))
                        {
                            if (_time0 is null)
                                _time0 = DateTime.Now;

                            if (_frameCount - _oldFrameCount > BufferSize)
                                IsSkipping = true;

                            _oldFrameCount = _frameCount;

                            WriteCaret = _newestFrameIndex;

                            
                            FrameCount = _frameCount;

                            while (ReadCaret != WriteCaret)
                            {
                                ReadCaret++;

                                FramestampBundleID[ReadCaret] = currentFrameBundle;

                                captureInfoArray[BundleCaret++] = new(
                                     ReadCaret,
                                     FramestampBuffer[ReadCaret][0] + FramestampBundleID[ReadCaret] * 65536,
                                     FramestampBuffer[ReadCaret][0] + FramestampBundleID[ReadCaret] * 65536 + BufferSize - 1,
                                     FramesBehind);

                                timestampsForEstimate[framerateEstimateCaret] = GetDateTimeFromTimestamp(ReadCaret).Ticks;
                                framerateEstimateCaret = (framerateEstimateCaret + 1) % NumFramesForFramerateEstimation;


                                if (BundleCaret == 0)
                                {
                                    frameEventsQueue.Enqueue(new(captureInfoArray, outputFrameID, FramesBehind / (int)OutputBundleSize));
                                    OnNewFrame?.Invoke(this, new OnNewFrameEventArgs(outputFrameID, outputFrameID+BufferSize/OutputBundleSize, FramesBehind/(int)OutputBundleSize, OutputBundleSize));
                                    outputFrameID++;
                                }

                                if (FramestampBuffer[ReadCaret][0] == 65535)
                                    currentFrameBundle++;
                            }

                        }
                    }

                    if (newEvent.Stopped)
                        break;
                }
                else
                {
                    if (api.LastError == HamamatsuError.Abort)
                        break;
                }
            }

            Dispose();
        }


        public int GetFrame(out HamamatsuSourceFrame? frame)
        {
            if (frameEventsQueue.TryDequeue(out BundleCaptureInfo? queueOut))
            {

                ConsumeCaret = queueOut.CollectionIDs[^1];

                frame = new HamamatsuSourceFrame(queueOut.CollectionIDs.Select(x => FrameBuffer[x]),
                                                 StatusChecker,
                                                 Converters.PixelTypeToPixelFormatConverter(PixelType),
                                                 (int)HSize,
                                                 (int)VSize,
                                                 (int)OutputBundleSize,
                                                 (int)queueOut.FrameID,
                                                 (int)queueOut.ValidUntilFrame,
                                                 queueOut.CollectionIDs.Select(x => GetDateTimeFromTimestamp(x)),
                                                 metadata.CameraID.ToString(),
                                                 metadata.Model
                                                 );

                return FramesBehind / (int)OutputBundleSize;
            }

            frame = null;
            return 0;
        }

        private DateTime GetDateTimeFromTimestamp(int x) => _time0!.Value.AddSeconds(TimestampBuffer[x][0].Sec).AddMicroseconds(TimestampBuffer[x][0].Microsec);

        void ReleaseGCHandles()
        {
            foreach (GCHandle hh in frameHandles)
                if (hh.IsAllocated) hh.Free();
            foreach (GCHandle hh in timestampHandles)
                if (hh.IsAllocated) hh.Free();
            foreach (GCHandle hh in framestampHandles)
                if (hh.IsAllocated) hh.Free();

            mainFrameHandle.Free();
            mainTimestampHandle.Free();
            mainFramestampHandle.Free();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _tokenSource.Cancel();
                    ReleaseGCHandles();
                }

                api.ReleaseBuffer();
                FrameBuffer = Array.Empty<byte[]>();
                disposedValue = true;
            }
        }

        ~HamamatsuRingBuffer()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

}
