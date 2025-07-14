using System.Runtime.InteropServices;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    [StructLayout(LayoutKind.Sequential,Pack=4)]
    public struct DCAMCaptureTransferInfo
    {
        public int Size;                      // [in] size of this structure.
        public int Reserved;                  // [in]
        public int NewestFrameIndex;         // [out]
        public int FrameCount;               // [out]

        public DCAMCaptureTransferInfo()
        {
            Size = Marshal.SizeOf(typeof(DCAMCaptureTransferInfo));
            Reserved = 0;
            NewestFrameIndex = 0;
            FrameCount = 0;
        }
    }
}