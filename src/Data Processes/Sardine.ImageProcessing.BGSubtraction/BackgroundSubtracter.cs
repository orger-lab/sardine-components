namespace Sardine.ImageProcessing.BGSubtraction
{
    /// <summary>
    /// Abstract class that implements the necessary methods to do background subtraction.
    /// </summary>
    public abstract class BackgroundSubtracter
    {

        public IImageFrame? BackgroundModel
        {
            get; protected set;
        }

        public IImageFrame? CurrentElement
        {
            get; set;
        }

        public bool UpdateBackgroundOverTime { get; set; } = true;
        public int BackgroundUpdatePeriod { get; set; } = 1000;
        public double BackgroundUpdateAlpha { get; set; } = 0.9;

        public BackgroundSubtracter() { }

        public void UpdateBackground()
        {
            BackgroundModel = UpdateBackgroundModel(CurrentElement, false);
        }

        public void RegularBackgoundUpdate()
        {
            BackgroundModel = UpdateBackgroundModel(CurrentElement, true);
        }


        /// <summary>
        /// Defines how the background model is updated.
        /// </summary>
        /// <param name="frame">Frame to be used in the background model calculation.</param>
        /// <returns>A new background model.</returns>
        protected abstract IImageFrame? UpdateBackgroundModel(IImageFrame? frame, bool regularUpdate);

        /// <summary>
        /// Defines how the background subtraction is prefromed on the input image.
        /// </summary>
        /// <param name="frame">Image to be subtracted.</param>
        /// <returns>The subracted image.</returns>
        public abstract IImageFrame? SubtractBackground(IImageFrame frame);
    }
}
