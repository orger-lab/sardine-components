namespace Sardine.Devices.Hamamatsu.Camera
{
    public class OnNewFrameEventArgs : EventArgs
    {
        public uint BundleSize { get; }
        public long FrameID { get; }
        public long ValidUntilFrame { get; }
        public int FramesBehind { get; }

        internal OnNewFrameEventArgs(long id, long validUntilFrame, int framesBehind, uint bundleSize)
        {
            FrameID = id;
            ValidUntilFrame = validUntilFrame;
            FramesBehind = framesBehind;
        }
    }
}
