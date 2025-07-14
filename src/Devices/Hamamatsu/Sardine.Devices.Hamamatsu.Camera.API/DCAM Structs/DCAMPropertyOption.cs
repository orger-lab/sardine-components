using System;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    public struct DCAMPropertyOption : IEquatable<DCAMPropertyOption>
    {
        private readonly uint _propoption;

        #region Property codes
        public static readonly DCAMPropertyOption PRIOR         = new(0xFF000000);   // prior value
        public static readonly DCAMPropertyOption NEXT          = new(0x01000000);   // next value or id
        public static readonly DCAMPropertyOption SUPPORT       = new(0x00000000);   // default option
        public static readonly DCAMPropertyOption UPDATED       = new(0x00000001);   // UPDATED and VOLATILE can be used at same time
        public static readonly DCAMPropertyOption VOLATILE      = new(0x00000002);   // UPDATED and VOLATILE can be used at same time
        public static readonly DCAMPropertyOption ARRAYELEMENT  = new(0x00000004);   // ARRAYELEMENT
        public static readonly DCAMPropertyOption NONE          = new(0x00000000);   // no option
        #endregion

        public DCAMPropertyOption(uint v)
        {
            _propoption = v;
        }

        #region IEquatable
        public bool Equals(DCAMPropertyOption a) => _propoption == a._propoption;
        
        public override bool Equals(object? obj)
        {
            if (obj is not DCAMPropertyOption)
                return false;

            return Equals((DCAMPropertyOption)obj);
        }
        #endregion

        public static implicit operator uint(DCAMPropertyOption self) => self._propoption;
        
        public static implicit operator int(DCAMPropertyOption self) => (int)self._propoption;

        public override int GetHashCode() => _propoption.GetHashCode();

        public static bool operator ==(DCAMPropertyOption left, DCAMPropertyOption right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DCAMPropertyOption left, DCAMPropertyOption right)
        {
            return !(left == right);
        }
    }
}