using System.Runtime.InteropServices;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    [StructLayout(LayoutKind.Sequential,Pack=4)]
    public struct DCAMRecordStatus
    {
        public int Size;
        public int CurrentSessionID;
        public int MaxFramecountSession;
        public int CurrentFrameID;
        public int MissingFrameCount;
        public int Flags;                     // DCAMREC_STATUSFLAG
        public int TotalFramecount;
        public int Reserved;
    }
}