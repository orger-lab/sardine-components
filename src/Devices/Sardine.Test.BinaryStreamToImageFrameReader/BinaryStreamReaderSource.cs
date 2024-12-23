using Sardine.ImageProcessing;

namespace Sardine.Test.BinaryStreamToImageFrameReader
{
    public static class BinaryStreamReaderSource
    {
        public static IImageFrame? SourceFrame(BinaryStreamReader reader, out bool hasMore)
        {
            hasMore = false;

            return reader.ReadNextFrame();
        }
    }
}
