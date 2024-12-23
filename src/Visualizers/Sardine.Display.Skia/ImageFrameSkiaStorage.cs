using System.Runtime.InteropServices;

namespace Sardine.Display.Skia
{
    public class ImageFrameSkiaStorage : IDisposable
    {
        private bool disposedValue;
        private byte[]? data;

        public ref byte[] Data => ref data!;

        public int MinValue { get; set; } = 0;
        public int MaxValue { get; set; } = 65535;

        public GCHandle Handle { get; }
        public int StorageSize { get; }

        public ImageFrameSkiaStorage(int storageSize)
        {
            Data = new byte[storageSize];
            StorageSize = storageSize;
            Handle = GCHandle.Alloc(Data, GCHandleType.Pinned);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Handle.Free();
                data = null;
                disposedValue = true;
            }
        }

        ~ImageFrameSkiaStorage()
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
