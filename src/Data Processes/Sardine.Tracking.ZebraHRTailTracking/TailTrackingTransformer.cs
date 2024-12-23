using Sardine.Core.DataModel;
using Sardine.ImageProcessing;

namespace Sardine.Tracking.ZebraHRTailTracking
{
    public partial class MPTailTracking
    {
        public static MPTailTrackingResult? Transform(MPTailTracking tracker, IImageFrame? frame, MessageMetadata metadata)
        {
            if (frame is null || !frame.IsAlive)
                return null;
            
            return tracker.GetTailPositionFromImage(frame.FrameData![0], (uint)frame.Width, (uint)frame.Height, frame.FrameID);
        }
    }
}
