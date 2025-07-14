namespace Sardine.Devices.Hamamatsu.Camera
{
    public class FrameCaptureInfo
    {
        public int CollectionID { get; }
        public long FrameID { get; }
        public long ValidUntilFrame { get; }
        public int FramesBehind { get; }

        internal FrameCaptureInfo(int collectionID, long id, long validUntilFrame, int framesBehind)
        {
            CollectionID = collectionID;
            FrameID = id;
            ValidUntilFrame = validUntilFrame;
            FramesBehind = framesBehind;
        }
    }


}
