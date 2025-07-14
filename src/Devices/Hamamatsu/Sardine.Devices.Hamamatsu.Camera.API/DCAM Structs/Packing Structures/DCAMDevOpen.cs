using System;
using System.Runtime.InteropServices;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    [StructLayout(LayoutKind.Sequential,Pack=4)]
    public struct DCAMDevOpen
    {
        public int Size;
        public int Index;
        public IntPtr HDCam;

        public DCAMDevOpen(int iCamera)
        {
            Size = Marshal.SizeOf(typeof(DCAMDevOpen));
            Index = iCamera;
            HDCam = IntPtr.Zero;
        }
    }
}