using System;
using System.Runtime.InteropServices;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    [StructLayout(LayoutKind.Sequential,CharSet=CharSet.Ansi)]
    public struct DCAMPropertyValueText
    {
        public int Size;                    // [in] size of this structure
        public int PropID;                     // [in] DCAMIDPROP
        public double Value;                    // [in] value of property
        public IntPtr Text;                     // [in, obuf] text of the value 
        public int TextBytes;                 // [in] text buf size

        public DCAMPropertyValueText(int _iprop)
        {
            Size = Marshal.SizeOf(typeof(DCAMPropertyValueText));
            PropID = _iprop;
            Value = 0;
            Text = IntPtr.Zero;
            TextBytes = 0;
        }
        public DCAMPropertyValueText(int _iprop, double _value)
        {
            Size = Marshal.SizeOf(typeof(DCAMPropertyValueText));
            PropID = _iprop;
            Value = _value;
            Text = IntPtr.Zero;
            TextBytes = 0;
        }
    }
}