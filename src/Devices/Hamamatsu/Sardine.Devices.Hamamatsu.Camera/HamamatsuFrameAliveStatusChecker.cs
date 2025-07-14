using Sardine.ImageProcessing;

namespace Sardine.Devices.Hamamatsu.Camera
{
    public class HamamatsuFrameAliveStatusChecker : IAliveStatusChecker
    {
        readonly HamamatsuRingBuffer buffer;

        public bool GetAliveStatus(int frameID)
        {
            bool response = buffer.FrameCount - frameID*buffer.OutputBundleSize < buffer.BufferSize;

            if (response == false)
                Console.WriteLine($"{buffer.FrameCount} - {frameID}");

            return response;
        }

        public HamamatsuFrameAliveStatusChecker(HamamatsuRingBuffer buffer)
        {
            this.buffer = buffer;
        }
    }

}
