namespace Sardine.Devices.Hamamatsu.Camera
{
    public class BundleCaptureInfo
    {
        public BundleCaptureInfo(FrameCaptureInfo[] frameInfos, long frameID, long validUntilFrame)
        {
            FrameID = frameID;

            int[] collectionIDs = new int[frameInfos.Length];

            for (int i = 0; i < frameInfos.Length; i++)
            {
                collectionIDs[i] = frameInfos[i].CollectionID;
            }
            CollectionIDs = collectionIDs;
            ValidUntilFrame = frameID;
        }

        public IReadOnlyList<int> CollectionIDs { get; }
        public long FrameID { get; }
        public long ValidUntilFrame { get; }
    }


}
