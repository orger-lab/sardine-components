namespace Sardine.ImageProcessing
{
    public static class PixelFormatExtensions
    {
        public static int GetBitsPerPixel(this PixelFormat pixelFormat)
        {
            return pixelFormat switch
            {
                PixelFormat.Mono => 1,
                PixelFormat.Gray8 => 8,
                PixelFormat.Gray12 => 12,
                PixelFormat.Gray16 => 16,
                PixelFormat.Rgba32 => 32,
                PixelFormat.Bgra32 => 32,
                PixelFormat.Rgba64 => 64,
                PixelFormat.Bgra64 => 64,
                PixelFormat.Rgb24 => 24,
                PixelFormat.Bgr24 => 24,
                _ => 0,
            };
        }
    }
}
