using System.Runtime.InteropServices;
using OpenCV.Net;

namespace Sardine.ImageProcessing.BGSubtraction.OpenCV
{

    public abstract class OpenCVBackgroundSubtracter : BackgroundSubtracter
    {
        protected IplImage? backgroundImage;
        IplImage? inputImage;
        IplImage? subtractedImage;

        byte[] backgroundImageData = [];

        private byte _ksize = 3;
        /// <summary>
        /// Kernel size.
        /// </summary>
        public byte KSize
        {
            get => _ksize;
            set
            {
                // Smallest kernel size value is 3 and it has to be an odd number.
                byte valToUpdate = (byte)Math.Max(3, value - 1 + (value % 2));

                if (_ksize != valToUpdate)
                {
                    _ksize = valToUpdate;

                    UpdateBackground();
                }
            }
        }

        /// <summary>
        /// Creates a new instance of a Background subtracter.
        /// </summary>
        /// <param name="width">Width of the background image.</param>
        /// <param name="height">Height of the background image.</param>
        public OpenCVBackgroundSubtracter()
        {
            var a= new IplImage(new Size(1, 1), IplDepth.U8, 1);
            a.Dispose();
        }

        /// <summary>
        /// Creates a new instance of a Background subtracter.
        /// </summary>
        /// <param name="width">Width of the background image.</param>
        /// <param name="height">Height of the background image.</param>
        /// <param name="modules">Subset of modules to subscribe to.</param>
        

        /// <summary>
        /// Creates all necessary <see cref="IplImage"/> for the wrapper.
        /// </summary>
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        private bool CreateIplImages(int width, int height)
        {
            if (backgroundImage is not null && backgroundImage.Width == width && backgroundImage.Height == height)
                return false;

            var imageSize = new Size(width, height);

            backgroundImage = new IplImage(imageSize, IplDepth.U8, 1);
            inputImage = new IplImage(imageSize, IplDepth.U8, 1);
            subtractedImage = new IplImage(imageSize, IplDepth.U8, 1);

            backgroundImageData = new byte[width * height];

            UpdateBackground();

            return true;
        }


            
        protected override ImageFrame? UpdateBackgroundModel(IImageFrame? frame, bool regularUpdate)
        {
            if (frame is null)
                return null;

            if (frame.PixelFormat != PixelFormat.Gray8)
                return null;

            CreateIplImages(frame.Width, frame.Height);

            unsafe
            {
                fixed (byte* ptr = frame.FrameData![0])
                {
                    IplImage inputImg = new IplImage(new Size(frame.Width, frame.Height), IplDepth.U8, 1, (IntPtr)ptr);
                    UpdateBackgroundModel(ref inputImg, ref backgroundImage!, regularUpdate);
                    Marshal.Copy(backgroundImage.ImageData, backgroundImageData, 0, frame.Width * frame.Height);
                    inputImg.Close();
                }
            }

            return new ImageFrame([backgroundImageData], frame.PixelFormat, frame.Timestamps, frame.UnitXSize, frame.UnitYSize, frame.UnitZSize, frame.Width, frame.Height, frame.Planes, true, frame.FrameID, frame.CameraSN, frame.CameraModel);
        }


        public override ImageFrame? SubtractBackground(IImageFrame? frame)
        {
            if (frame is null)
                return null;

            if (frame.PixelFormat != PixelFormat.Gray8)
                return null;


            CreateIplImages(frame.Width, frame.Height);
            var imageData = new byte[frame.Width * frame.Height];

            unsafe
            {
                fixed (byte* ptr = frame.FrameData![0])
                {
                    inputImage!.SetData((IntPtr)ptr, frame.Width);
                    SubtractBackground(inputImage, backgroundImage, ref subtractedImage);
                    Marshal.Copy(subtractedImage.ImageData, imageData, 0, frame.Width * frame.Height);
                }
            }

            return new ImageFrame([imageData], frame.PixelFormat, frame.Timestamps, frame.UnitXSize, frame.UnitYSize, frame.UnitZSize, frame.Width, frame.Height, frame.Planes, true, frame.FrameID, frame.CameraSN, frame.CameraModel);
        }

        /// <summary>
        /// Defines how the background model is updated.
        /// </summary>
        /// <param name="inputImage">Image to be used in the background model calculation.</param>
        /// <param name="backgroundModel">The resulting background model.</param>
        protected abstract void UpdateBackgroundModel(ref IplImage inputImage, ref IplImage backgroundModel, bool regularUpdate);

        /// <summary>
        /// Defines how the background subtraction is prefromed on the input image.
        /// </summary>
        /// <param name="inputImage">Image to be subtracted.</param>
        /// <param name="backgroundModel">Background model that will interact with the input image.</param>
        /// <param name="subtractedImage">Resulting image after subtraction.</param>
        protected abstract void SubtractBackground(in IplImage inputImage, in IplImage backgroundModel, ref IplImage subtractedImage);
    }
}