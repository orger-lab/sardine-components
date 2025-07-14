using Sardine.Devices.Hamamatsu.Camera.API;

namespace Sardine.Devices.Hamamatsu.Camera
{
    public class OnStatusChangedEventArgs : EventArgs
    {
        public CaptureStatus NewStatus { get; private set; }

        internal OnStatusChangedEventArgs(CaptureStatus newStatus)
        {
            NewStatus = newStatus;
        }
    }
}
