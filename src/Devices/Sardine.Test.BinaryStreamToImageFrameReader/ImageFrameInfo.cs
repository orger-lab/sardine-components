namespace Sardine.Test.BinaryStreamToImageFrameReader
{
    public partial class BinaryStreamReader
    {
        public class ImageFrameInfo
        {
            public ImageFrameInfo(double unitXSize, double unitYSize, double unitZSize, int width, int height, int planes, string cameraModel, string cameraSN, int frameSize, int headerSize)
            {
                UnitXSize = unitXSize;
                UnitYSize = unitYSize;
                UnitZSize = unitZSize;
                Width = width;
                Height = height;
                Planes = planes;
                CameraSN = cameraSN;
                CameraModel = cameraModel;
                FrameSize = frameSize;
                HeaderSize = headerSize;
            }

            public double UnitXSize { get; }
            public double UnitYSize { get; }
            public double UnitZSize { get; }
            public int Width { get; }
            public int Height { get; }
            public int Planes { get; }
            public string CameraSN { get; }
            public string CameraModel { get; }
            public int FrameSize { get; }

            public int HeaderSize { get; } 
        }
    }
}
