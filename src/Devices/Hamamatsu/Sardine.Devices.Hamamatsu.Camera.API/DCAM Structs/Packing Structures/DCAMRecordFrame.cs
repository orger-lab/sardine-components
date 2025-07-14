using System;
using System.Runtime.InteropServices;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    [StructLayout(LayoutKind.Sequential,Pack=4)]
    public struct DCAMRecordFrame
    {
        public int Size;                      // [i] size of this structure.
        public int KindID;                     // [i] reserved. set to 0.
        public int Option;                    // [i] reserved. set to 0.
        public int FrameID;                    // [i] reserved. set to 0.
        public IntPtr Buffer;                      // [o] pointer for top-left image
        public int RowBytes;                  // [o] byte size for next line.
        public PixelType Type;             // [o] return pixeltype of image. set to 0 before call.
        public int Width;                     // [o] horizontal pixel count
        public int Height;                    // [o] vertical line count
        public int Left;                      // [o] horizontal start pixel
        public int Top;                       // [o] vertical start line
        public DCAMTimeStamp Timestamp;        // [o] timestamp
        public int Framestamp;                // [o] framestamp
        public int Camerastamp;               // [o] camerastamp

        public DCAMRecordFrame(int indexFrame)
        {
            Size = Marshal.SizeOf(typeof(DCAMRecordFrame));
            KindID = 0;
            Option = 0;
            FrameID = indexFrame;
            Buffer = IntPtr.Zero;
            RowBytes = 0;
            Type = PixelType.None;
            Width = 0;
            Height = 0;
            Left = 0;
            Top = 0;
            Timestamp.Sec = 0;
            Timestamp.Microsec = 0;
            Framestamp = 0;
            Camerastamp = 0;
        }
    }
}