namespace Sardine.ImageProcessing
{
    public interface IImageFrame
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
    }
}
