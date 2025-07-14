using System;
using System.Runtime.InteropServices;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    [StructLayout(LayoutKind.Sequential,CharSet=CharSet.Ansi)]
    public struct DCAMDevString
    {
        public int Size;                      // [in]
        public int StringID;                   // [in]
        public IntPtr Text;                     // [in,obuf]
        public int TextBytes;                 // [in]

        public DCAMDevString(int istr)
        {
            Size = Marshal.SizeOf(typeof(DCAMDevString));
            StringID = istr;
            Text = IntPtr.Zero;
            TextBytes = 0;
        }
    }
}