using System.Runtime.InteropServices;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    // ##########################
    // ####  Structs
    // ##########################
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DCAMTimeStamp
    {
        public uint Sec;                      // [out]
        public int Microsec;                  // [out]
    }
}