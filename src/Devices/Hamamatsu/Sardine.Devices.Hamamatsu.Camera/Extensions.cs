using Sardine.ImageProcessing;
using Sardine.Devices.Hamamatsu.Camera.API;

namespace Sardine.Devices.Hamamatsu.Camera
{
    public static class Converters
    {
        public static byte[] UshortToByteArrayConverter(ushort[] dataIn)
        {
            if (dataIn == null)
                return Array.Empty<byte>();

            byte[] dataOut = new byte[dataIn.Length * 2];
            Buffer.BlockCopy(dataIn, 0, dataOut, 0, dataOut.Length);
            return dataOut;
        }

        public static byte[] ByteToByteArray(byte[] dataIn) => dataIn;

        public static PixelFormat PixelTypeToPixelFormatConverter(PixelType type)
        {
            return type switch
            {
                PixelType.Mono8 => PixelFormat.Gray8,
                PixelType.Mono16 => PixelFormat.Gray16,
                PixelType.Mono12 => PixelFormat.Gray12,
                PixelType.Mono12P => PixelFormat.Gray12,
                PixelType.RGB24 => PixelFormat.Rgb24,
                PixelType.BGR24 => PixelFormat.Bgr24,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
