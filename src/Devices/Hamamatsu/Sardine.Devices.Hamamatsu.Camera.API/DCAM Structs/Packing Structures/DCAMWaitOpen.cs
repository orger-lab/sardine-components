using System;
using System.Runtime.InteropServices;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    [StructLayout(LayoutKind.Sequential,Pack=4)]
    public struct DCAMWaitOpen
    {
        public int Size;                      // [in] size of this structure.
        public int SupportEvent;              // [out];
        public IntPtr HWait;                    // [out];
        public IntPtr HDCam;                    // [in];

        public DCAMWaitOpen()
        {
            Size = Marshal.SizeOf(typeof(DCAMWaitOpen));
            SupportEvent = 0;
            HWait = IntPtr.Zero;
            HDCam = IntPtr.Zero;
        }
    }
}