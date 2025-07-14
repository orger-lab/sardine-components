using Sardine.Devices.Hamamatsu.Camera.API;


namespace Sardine.Devices.Hamamatsu.Camera
{
    public class OnCaptureEventEventArgs : EventArgs
    {
        public List<CaptureEvent> Events { get; private set; }

        private readonly DCAMWait _event = DCAMWait.NONE;

        public bool CycleEnd => _event & DCAMWait.CAPEVENT.CYCLEEND;
        public bool ExposureEnd => _event & DCAMWait.CAPEVENT.EXPOSUREEND;
        public bool FrameReady => _event & DCAMWait.CAPEVENT.FRAMEREADY;
        public bool Stopped => _event & DCAMWait.CAPEVENT.STOPPED;
        public bool Transferred => _event & DCAMWait.CAPEVENT.TRANSFERRED;

        public OnCaptureEventEventArgs(CaptureEvent evnt) : this(new List<CaptureEvent>() { evnt }) { }

        public OnCaptureEventEventArgs(List<CaptureEvent> events)
        {
            Events = events;
            foreach (CaptureEvent ev in events)
            {
                switch (ev)
                {
                    case CaptureEvent.CycleEnd:
                        _event ^= DCAMWait.CAPEVENT.CYCLEEND;
                        break;
                    case CaptureEvent.ExposureEnd:
                        _event ^= DCAMWait.CAPEVENT.EXPOSUREEND;
                        break;
                    case CaptureEvent.FrameReady:
                        _event ^= DCAMWait.CAPEVENT.FRAMEREADY;
                        break;
                    case CaptureEvent.Stopped:
                        _event ^= DCAMWait.CAPEVENT.STOPPED;
                        break;
                    case CaptureEvent.Transferred:
                        _event ^= DCAMWait.CAPEVENT.TRANSFERRED;
                        break;
                }
            }
        }

        internal OnCaptureEventEventArgs(DCAMWait evnt)
        {
            _event = evnt;
            List<CaptureEvent> events = new(6);

            if (evnt & DCAMWait.CAPEVENT.CYCLEEND)
                events.Add(CaptureEvent.CycleEnd);
            if (evnt & DCAMWait.CAPEVENT.EXPOSUREEND)
                events.Add(CaptureEvent.ExposureEnd);
            if (evnt & DCAMWait.CAPEVENT.FRAMEREADY)
                events.Add(CaptureEvent.FrameReady);
            if (evnt & DCAMWait.CAPEVENT.STOPPED)
                events.Add(CaptureEvent.Stopped);
            if (evnt & DCAMWait.CAPEVENT.TRANSFERRED)
                events.Add(CaptureEvent.Transferred);

            Events = events;
        }
    }
}
