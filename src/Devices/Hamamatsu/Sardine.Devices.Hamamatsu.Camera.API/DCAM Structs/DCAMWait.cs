using System;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    public struct DCAMWait : IEquatable<DCAMWait>
    {
        private readonly int _eventbit;

        #region Property codes & structs
        public static readonly DCAMWait NONE = new(0);

        public bool CycleEnd => this & CAPEVENT.CYCLEEND;
        public bool ExposureEnd => this & CAPEVENT.EXPOSUREEND;
        public bool FrameReady => this & CAPEVENT.FRAMEREADY;
        public bool Stopped => this & CAPEVENT.STOPPED;
        public bool Transferred => this & CAPEVENT.TRANSFERRED;
        public struct CAPEVENT
        {
            public static readonly DCAMWait TRANSFERRED = new(0x0001);
            public static readonly DCAMWait FRAMEREADY  = new(0x0002);
            public static readonly DCAMWait CYCLEEND    = new(0x0004);
            public static readonly DCAMWait EXPOSUREEND = new(0x0008);
            public static readonly DCAMWait STOPPED     = new(0x0010);

        };

        public struct RECEVENT
        {
            public static readonly DCAMWait STOPPED     = new(0x0100);
            public static readonly DCAMWait WARNING     = new(0x0200);
            public static readonly DCAMWait MISSED      = new(0x0400);
            public static readonly DCAMWait DISKFULL    = new(0x1000);
            public static readonly DCAMWait WRITEFAULT  = new(0x2000);
            public static readonly DCAMWait SKIPPED     = new(0x4000);
            public static readonly DCAMWait WRITEFRAME  = new(0x8000);

        };
        #endregion

        public DCAMWait(int v)
        {
            _eventbit = v;
        }

        #region IEquatable
        public bool Equals(DCAMWait a) => _eventbit == a._eventbit;
        
        public override bool Equals(object? obj)
        {
            if (obj is not DCAMWait)
                return false;

            return Equals((DCAMWait)obj);
        }
        #endregion

        #region Operators
        public static implicit operator uint(DCAMWait self) => (uint)self._eventbit;
        
        public static implicit operator int(DCAMWait self) => self._eventbit;
        
        public static implicit operator bool(DCAMWait self) => self._eventbit != 0;
        
        public static DCAMWait operator |(DCAMWait obj, int value) => new(obj._eventbit | value);
        
        public static DCAMWait operator &(DCAMWait obj, int value) => new(obj._eventbit & value);
        
        public static DCAMWait operator ^(DCAMWait obj, int value) => new(obj._eventbit ^ value);
        #endregion

        public override int GetHashCode() => _eventbit.GetHashCode();

        public static bool operator ==(DCAMWait left, DCAMWait right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DCAMWait left, DCAMWait right)
        {
            return !(left == right);
        }
    }
}