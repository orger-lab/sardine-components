using OpenCV.Net;


namespace Sardine.ImageProcessing.BGSubtraction.OpenCV
{
    /// <summary>
    /// Calculates a background subtraction using the absolute value of the difference between an image and a background model via OpenCV methods.
    /// </summary>
    /// <typeparam name="T"> Type of data to subscribe</typeparam>
    public class AbsoluteDifference : OpenCVBackgroundSubtracter
    {

        protected override void UpdateBackgroundModel(ref IplImage inputImage, ref IplImage backgroundModel, bool regularUpdate)
        {
            CV.Smooth(inputImage, backgroundModel, SmoothMethod.Median, KSize);
            Console.WriteLine("Updated Background Model");
        }

        protected override void SubtractBackground(in IplImage inputImage, in IplImage backgroundModel, ref IplImage subtractedImage)
        {
            CV.AbsDiff(inputImage, backgroundModel, subtractedImage);
        }

    }
}
