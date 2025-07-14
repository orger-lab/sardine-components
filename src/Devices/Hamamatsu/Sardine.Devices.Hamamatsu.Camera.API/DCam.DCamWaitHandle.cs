using System;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    public partial class DCam
    {
        public class DCamWaitHandle : IDisposable
        {
            private IntPtr _hWait = IntPtr.Zero;
            private bool disposedValue;
            private readonly DCam _dCam;

            internal DCamWaitHandle(DCam dCam)
            {
                _dCam = dCam;

                if (!_dCam.IsConnected)
                    return;

                DCAMWaitOpen param = new()
                {
                    HDCam = _dCam._hdcam
                };

                _dCam.LastError = _dCam.api.OpenWait(ref param);
                if (_dCam.LastError.Failed())
                    return;

                _hWait = param.HWait;
            }

            public bool Wait(DCAMWait eventMask, ref DCAMWait eventHappened, int timeout = unchecked((int)0x80000000))
            {
                if (_hWait == IntPtr.Zero)
                    return false;

                DCAMWaitStart param = new(eventMask, timeout);
                _dCam.LastError = _dCam.api.StartWait(_hWait, ref param);
                if (!_dCam.LastError.Failed())
                    eventHappened = new DCAMWait(param.EventHappened);

                return !_dCam.LastError.Failed();
            }

            public bool AbortWait()
            {
                if (_hWait == IntPtr.Zero)
                    return false;

                _dCam.LastError = _dCam.api.AbortWait(_hWait);

                return !_dCam.LastError.Failed();
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        _dCam.api.CloseWait(_hWait);
                        _hWait = IntPtr.Zero;
                    }
                    disposedValue = true;
                }
            }

            public void Dispose()
            {
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }
    }
}
