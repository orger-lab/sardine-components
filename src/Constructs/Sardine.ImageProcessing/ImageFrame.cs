namespace Sardine.ImageProcessing
{
    public class ImageFrame : IImageFrame
    {
        
            public IReadOnlyList<byte[]>? FrameData { get; }
            public PixelFormat PixelFormat { get; }
            public IReadOnlyList<DateTime> Timestamps { get; }
            public double UnitXSize { get; }
            public double UnitYSize { get; }
            public double UnitZSize { get; }
            public int Width { get; }
            public int Height { get; }
            public int Planes { get; }
            public bool IsAlive { get; }
            public int FrameID { get; }
            public string CameraSN { get; }
            public string CameraModel { get; }

        public ImageFrame(IReadOnlyList<byte[]>? frameData, PixelFormat pixelFormat, IReadOnlyList<DateTime> timestamps, double unitXSize, double unitYSize, double unitZSize, int width, int height, int planes, bool isAlive, int frameID, string cameraSN, string cameraModel)
        {
            FrameData = frameData;
            PixelFormat = pixelFormat;
            Timestamps = timestamps ?? throw new ArgumentNullException(nameof(timestamps));
            UnitXSize = unitXSize;
            UnitYSize = unitYSize;
            UnitZSize = unitZSize;
            Width = width;
            Height = height;
            Planes = planes;
            IsAlive = isAlive;
            FrameID = frameID;
            CameraSN = cameraSN ?? throw new ArgumentNullException(nameof(cameraSN));
            CameraModel = cameraModel ?? throw new ArgumentNullException(nameof(cameraModel));
        }
    }
}
