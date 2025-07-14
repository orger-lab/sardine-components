using Sardine.ImageProcessing;
using Sardine.Recording.Data.Text;

namespace Sardine.Devices.Hamamatsu.Camera
{
    public class HamamatsuSourceFrame : IImageFrame, ITextWritable, IDisposable
    {
        private bool disposedValue;

        public IReadOnlyList<byte[]> FrameData { get; }
        public PixelFormat PixelFormat { get; }
        public int Width { get; }
        public int Height { get; }
        public int Planes { get; }
        public double UnitXSize { get; internal set; }
        public double UnitYSize { get; internal set; }
        public double UnitZSize { get; internal set; }
        public bool IsAlive => frameAliveStatusChecker.GetAliveStatus(FrameID);


        IAliveStatusChecker frameAliveStatusChecker { get; }
        public int AliveUntilID { get; }

        public string CameraSN { get; }

        public string CameraModel { get; }

        public int FrameID { get; }

        public IReadOnlyList<DateTime> Timestamps { get; }

        public HamamatsuSourceFrame(IEnumerable<byte[]> fd, IAliveStatusChecker frameAliveStatusChecker, PixelFormat pf, int w, int h, int bundleSize, int id, int aliveUntil, IEnumerable<DateTime> timestamp, string cameraSN, string cameraModel)
        {
            ArgumentNullException.ThrowIfNull(fd);

            FrameData = fd.ToList().AsReadOnly();
            this.frameAliveStatusChecker = frameAliveStatusChecker;
            PixelFormat = pf;
            Width = w;
            Height = h;
            FrameID = id;
            Planes = bundleSize;
            AliveUntilID = aliveUntil;
            Timestamps = timestamp.ToList().AsReadOnly();
            CameraModel = cameraModel;
            CameraSN = cameraSN;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public string WriteHeader()
        {
            return "ID Timestamp";
        }

        public string WriteData()
        {
            return $"{FrameID} {(double)Timestamps[^1].Ticks / TimeSpan.TicksPerSecond}";
        }
    }
}
