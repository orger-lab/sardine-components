using System.Runtime.InteropServices;

namespace Sardine.Devices.Hamamatsu.Camera.API
{ 
    [StructLayout(LayoutKind.Sequential,Pack=4)]
    public struct DCAMDevInit
    {
        public int Size;                      // [in]
        public int Reserved;                  // [in] reserved to 0
        public int CapFlag;                   // [out] DCAMDEV_CAPFLAG
        public int Reserved2;                 // [out] reserved to 0
    }
}