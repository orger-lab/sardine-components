using Sardine.Core.DataModel;

namespace Sardine.ImageProcessing.BGSubtraction
{
    public static class BackgroundSubtracterTransformer
    {
        public static IImageFrame? Transform(BackgroundSubtracter bgSubtracter, IImageFrame dataIn, MessageMetadata metadata)
        {
            bgSubtracter.CurrentElement = dataIn;

            if (metadata.SourceID % bgSubtracter.BackgroundUpdatePeriod == 0)
                bgSubtracter.RegularBackgoundUpdate();

            return bgSubtracter.SubtractBackground(dataIn);
        }
    }
}
