using System;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    public struct DCAMStringID : IEquatable<DCAMStringID>
    {
        private readonly uint _id;

        #region Property codes
        public static readonly DCAMStringID BUS                = new(0x04000101);
        public static readonly DCAMStringID CAMERAID           = new(0x04000102);
        public static readonly DCAMStringID VENDOR             = new(0x04000103);
        public static readonly DCAMStringID MODEL              = new(0x04000104);
        public static readonly DCAMStringID CAMERAVERSION      = new(0x04000105);
        public static readonly DCAMStringID DRIVERVERSION      = new(0x04000106);
        public static readonly DCAMStringID MODULEVERSION      = new(0x04000107);
        public static readonly DCAMStringID DCAMAPIVERSION     = new(0x04000108);
        public static readonly DCAMStringID CAMERA_SERIESNAME  = new(0x0400012c);
        #endregion

        #region Operators
        public static implicit operator uint(DCAMStringID self) => self._id;

        public static implicit operator int(DCAMStringID self) => (int)self._id;
        #endregion

        #region IEquatable
        public bool Equals(DCAMStringID a) => _id == a._id;

        public override bool Equals(object? obj)
        {
            if (obj is not DCAMStringID)
                return false;

            return Equals((DCAMStringID)obj);
        }
        #endregion

        public DCAMStringID(uint v)
        {
            _id = v;
        }

        public override int GetHashCode() => _id.GetHashCode();

        public static bool operator ==(DCAMStringID left, DCAMStringID right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DCAMStringID left, DCAMStringID right)
        {
            return !(left == right);
        }
    }
}
