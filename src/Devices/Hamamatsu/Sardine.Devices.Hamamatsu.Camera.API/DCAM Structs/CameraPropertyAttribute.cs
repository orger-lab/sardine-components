using System;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    public struct CameraPropertyAttribute : IEquatable<CameraPropertyAttribute>
    {
        private readonly uint _attribute;

        public static readonly CameraPropertyAttribute HASRANGE = new(0x80000000);
        public static readonly CameraPropertyAttribute HASSTEP = new(0x40000000);
        public static readonly CameraPropertyAttribute HASDEFAULT = new(0x20000000);
        public static readonly CameraPropertyAttribute HASVALUETEXT = new(0x10000000);
        public static readonly CameraPropertyAttribute HASCHANNEL = new(0x08000000);// value can set the value for each channels
        public static readonly CameraPropertyAttribute AUTOROUNDING = new(0x00800000);
        public static readonly CameraPropertyAttribute STEPPING_INCONSISTENT = new(0x00400000);
        public static readonly CameraPropertyAttribute DATASTREAM = new(0x00200000);// value is releated to image attribute
        public static readonly CameraPropertyAttribute HASRATIO = new(0x00100000);// value has ratio control capability
        public static readonly CameraPropertyAttribute VOLATILE = new(0x00080000);// value may be changed by user or automatically
        public static readonly CameraPropertyAttribute WRITABLE = new(0x00020000);// value can be set when state is manual
        public static readonly CameraPropertyAttribute READABLE = new(0x00010000);// value is readable when state is manual
        public static readonly CameraPropertyAttribute HASVIEW = new(0x00008000);// value can set the value for each views
        public static readonly CameraPropertyAttribute ACCESSREADY = new(0x00002000);// This value can get or set at READY status
        public static readonly CameraPropertyAttribute ACCESSBUSY = new(0x00001000);// This value can get or set at BUSY status
        public static readonly CameraPropertyAttribute TYPE_NONE = new(0x00000000);// undefined
        public static readonly CameraPropertyAttribute TYPE_MODE = new(0x00000001);// 01:  mode, 32bit integer in case of 32bit OS
        public static readonly CameraPropertyAttribute TYPE_LONG = new(0x00000002);// 02:  32bit integer in case of 32bit OS
        public static readonly CameraPropertyAttribute TYPE_REAL = new(0x00000003);// 03:  64bit float
        public static readonly CameraPropertyAttribute TYPE_MASK = new(0x0000000F);// mask for property value type

        internal CameraPropertyAttribute(uint v)
        {
            _attribute = v;
        }

        public static implicit operator uint(CameraPropertyAttribute self) => self._attribute;
        public static implicit operator int(CameraPropertyAttribute self) => (int)self._attribute;
        public static CameraPropertyAttribute operator |(CameraPropertyAttribute obj, uint value) => new(obj._attribute | value);
        public static CameraPropertyAttribute operator &(CameraPropertyAttribute obj, uint value) => new(obj._attribute & value);
        public static CameraPropertyAttribute operator ^(CameraPropertyAttribute obj, uint value) => new(obj._attribute ^ value);

        public bool Equals(CameraPropertyAttribute a) => _attribute == a._attribute;
        public override int GetHashCode() => _attribute.GetHashCode();
        public bool IsType(CameraPropertyAttribute type)
        {
            if ((_attribute & TYPE_MASK) == (int)type)
                return true;

            return false;
        }
        public bool HasAttribute(CameraPropertyAttribute attr)
        {
            if ((_attribute & (int)attr) == 0)
                return false;

            return true;
        }

        public override bool Equals(object? obj)
        {
            return obj is CameraPropertyAttribute attribute && Equals(attribute);
        }

        public static bool operator ==(CameraPropertyAttribute left, CameraPropertyAttribute right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CameraPropertyAttribute left, CameraPropertyAttribute right)
        {
            return !(left == right);
        }
    }
}