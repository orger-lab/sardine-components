using System;
using System.Runtime.InteropServices;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct DCAMRecordOpen
    {
        public int Size;                      // [in] size of this structure.
        public int Reserved;                  // [in]
        public IntPtr HRec;                     // [out]
        public string Path;                     // [in]
        public string Ext;                      // [in]
        public int MaxFramePerSession;        // [in]
        public int UserDataSize;              // [in]
        public int UserSessionDataSize;      // [in]
        public int UserFileDataSize;         // [in]
        public int UserTextSize;              // [in]
        public int UserSessionTextSize;      // [in]
        public int UserFileTextSize;         // [in]
    }
}