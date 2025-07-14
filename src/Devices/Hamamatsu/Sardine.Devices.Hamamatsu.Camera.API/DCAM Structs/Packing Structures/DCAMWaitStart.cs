using System.Runtime.InteropServices;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    [StructLayout(LayoutKind.Sequential,Pack=4)]
    public struct DCAMWaitStart
    {
        public int Size;                      // [in] size of this structure.
        public int EventHappened;             // [out]
        public int EventMask;                 // [in]
        public int Timeout;                   // [in]

        public DCAMWaitStart(int _mask, int timeout)
        {
            Size = Marshal.SizeOf(typeof(DCAMWaitStart));
            EventHappened = 0;
            EventMask = _mask;
            Timeout = timeout;
        }
    }
}