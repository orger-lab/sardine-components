using OpenCV.Net;

namespace Sardine.ImageProcessing.BGSubtraction.OpenCV
{
    /// <summary>
    /// Calculates a background subtraction using the absolute value of the difference between an image and a background model via OpenCV methods.
    /// </summary>
    /// <typeparam name="T"> Type of data to subscribe</typeparam>
    public class PositiveDiffWithMinBG : OpenCVBackgroundSubtracter
    {
        protected override void UpdateBackgroundModel(ref IplImage inputImage, ref IplImage backgroundModel, bool regularUpdate)
        {
            IplImage newBG = new IplImage(inputImage.Size, IplDepth.U8, 1);
            CV.Smooth(inputImage, newBG, SmoothMethod.Median, KSize);
            CV.Min(inputImage, newBG, newBG);
            
            if (regularUpdate)
            {
                backgroundModel = newBG * BackgroundUpdateAlpha + backgroundModel * (1 - BackgroundUpdateAlpha);
            }
            else
            {
                backgroundModel = newBG;
            }
        }

        protected override void SubtractBackground(in IplImage inputImage, in IplImage backgroundModel, ref IplImage subtractedImage)
        {
            CV.Sub(inputImage, backgroundModel, subtractedImage);
        }

    }
}
