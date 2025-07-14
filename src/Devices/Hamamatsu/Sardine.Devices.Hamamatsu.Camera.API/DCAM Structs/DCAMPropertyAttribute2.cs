using System;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    public struct DCAMPropertyAttribute2 : IEquatable<DCAMPropertyAttribute2>
    {
        private readonly uint _attribute;

        #region Property codes
        public static readonly DCAMPropertyAttribute2 ARRAYBASE = new(0x08000000);
        public static readonly DCAMPropertyAttribute2 ARRAYELEMENT = new(0x04000000);
        public static readonly DCAMPropertyAttribute2 REAL32 = new(0x02000000);
        public static readonly DCAMPropertyAttribute2 _FUTUREUSE = new(0x0007FFFC);
        #endregion

        #region Constructors
        public DCAMPropertyAttribute2(uint v)
        {
            _attribute = v;
        }
        public DCAMPropertyAttribute2(int v)
        {
            _attribute = (uint)v;
        }
        #endregion

        #region IEquatable
        public bool Equals(DCAMPropertyAttribute2 a) => _attribute == a._attribute;

        public override bool Equals(object? obj)
        {
            if (obj is not DCAMPropertyAttribute2)
                return false;

            return Equals((DCAMPropertyAttribute2)obj);
        }
        #endregion

        #region Operators
        public static implicit operator uint(DCAMPropertyAttribute2 self) => self._attribute;

        public static implicit operator int(DCAMPropertyAttribute2 self) => (int)self._attribute;

        public static DCAMPropertyAttribute2 operator |(DCAMPropertyAttribute2 obj, uint value) => new(obj._attribute | value);

        public static DCAMPropertyAttribute2 operator &(DCAMPropertyAttribute2 obj, uint value) => new(obj._attribute & value);

        public static DCAMPropertyAttribute2 operator ^(DCAMPropertyAttribute2 obj, uint value) => new(obj._attribute ^ value);
        #endregion

        public override int GetHashCode() => _attribute.GetHashCode();

        public bool HasAttribute(DCAMPropertyAttribute2 attr)
        {
            if ((_attribute & (int)attr) == 0)
                return false;

            return true;
        }

        public static bool operator ==(DCAMPropertyAttribute2 left, DCAMPropertyAttribute2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DCAMPropertyAttribute2 left, DCAMPropertyAttribute2 right)
        {
            return !(left == right);
        }
    }
}