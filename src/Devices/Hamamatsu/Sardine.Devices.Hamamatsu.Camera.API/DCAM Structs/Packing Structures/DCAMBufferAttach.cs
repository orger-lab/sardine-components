using System;
using System.Runtime.InteropServices;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    [StructLayout(LayoutKind.Sequential,Pack=8)]
    public struct DCAMBufferAttach
    {
        public int Size;                      // [in] size of this structure.
        public int KindID;                     // [in] DCAMBUF_ATTACHKIND
        public IntPtr Buffer;                   // [in,ptr]
        public int BufferCount;               // [in]
        
        public DCAMBufferAttach(int bufferCount, IntPtr buffer, AttachKind kind)
        {
            Size = Marshal.SizeOf(typeof(DCAMBufferAttach));
            KindID = (int)kind;
            Buffer = buffer;
            BufferCount = bufferCount;
        }
    }
}