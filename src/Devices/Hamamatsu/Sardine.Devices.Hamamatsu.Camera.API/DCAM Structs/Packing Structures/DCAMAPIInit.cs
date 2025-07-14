using System;
using System.Runtime.InteropServices;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    [StructLayout(LayoutKind.Sequential,Pack=4)]
    public struct DCAMAPIInit
    {
        public int Size;                      // [in]
        public int DeviceCountID;              // [out]
        public int Reserved;                  // reserved
        public int InitOptionBytes;           // [in] maximum bytes of initoption array.
        public IntPtr InitOption;               // [in ptr] initialize options. Choose from DCAMAPI_INITOPTION
        public IntPtr Guid;                     // [in ptr]

        public DCAMAPIInit()
        {
            Size = Marshal.SizeOf(typeof(DCAMAPIInit));
            DeviceCountID = 0;
            Reserved = 0;
            InitOption = IntPtr.Zero;
            InitOptionBytes = 0;
            Guid = IntPtr.Zero;
        }
    }
}