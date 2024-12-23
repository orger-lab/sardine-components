using Sardine.Core;
using Sardine.ImageProcessing;
using SkiaSharp;

namespace Sardine.Display.Skia
{
    public class SkiaSharpDisplayOptions
    {
        public bool AutoContrast { get; set; }
        public int MinContrastValue { get; set; } = 0;
        public int MaxContrastValue { get; set; } = 65535;
        public int DepthToShow { get; set; } = 0;

    }

    public sealed class SkiaSharpDisplay : MultiSourceDisplay<SKCanvas>
    {
        private uint viewportWidth = 600;
        private uint viewportHeight = 600;
        private bool autoContrast = false;
        private int minContrastValue = 0;
        private int maxContrastValue = 65535;
        private int depthToShow;

        public int MinContrastValue
        {
            get => minContrastValue; set
            {
                minContrastValue = value;
                ((SkiaSharpDisplayOptions)DisplayOptions).MinContrastValue = Math.Min(MaxContrastValue, Math.Max(0, MinContrastValue));
                OnPropertyChanged();
            }
        }
        public int MaxContrastValue
        {
            get => maxContrastValue; set
            {
                maxContrastValue = value;
                ((SkiaSharpDisplayOptions)DisplayOptions).MaxContrastValue = Math.Min(65535, Math.Max(MaxContrastValue, MinContrastValue));
                OnPropertyChanged();
            }
        }

        public int DepthToShow
        {
            get => depthToShow; set
            {
                depthToShow = Math.Max(0, value);
                ((SkiaSharpDisplayOptions)DisplayOptions).DepthToShow = depthToShow;
                OnPropertyChanged();
            }
        }

        public int MaxDepth
        {
            get;set;
        }


        public bool AutoContrast
        {
            get => autoContrast;
            set
            {
                autoContrast = value;
                ((SkiaSharpDisplayOptions)DisplayOptions).AutoContrast = autoContrast;
                OnPropertyChanged();
            }
        }

        public void SetContrast()
        {
            foreach (var obj in _nextToDraw)
            {
                if (obj is ImageFrameSkia frameStorage)
                {
                    double sum = 0;
                    double squareSum = 0;
                    int value = 0;
                    var depth = (DepthToShow<frameStorage.Planes)?DepthToShow: 0;

                    switch (frameStorage.PixelFormat)
                    {
                        case PixelFormat.Gray16:
                            {
                                for (int i = 0; i < frameStorage.FrameData[depth].Length / 2; i++)
                                {
                                    value = DrawingExtensions.From16BitArray(frameStorage.FrameData[depth], i*2);
                                    sum += value;
                                    squareSum += value * value;
                                }

                                double mean = sum / (frameStorage.FrameData[depth].Length / 2);
                                double stdev = Math.Sqrt((squareSum / (frameStorage.FrameData[depth].Length / 2)) - (mean * mean));

                                MinContrastValue = (int)Math.Max(0, (mean - 2 * stdev));
                                MaxContrastValue = (int)Math.Min(65535, (mean + 2 * stdev));
                            }
                            break;
                        case PixelFormat.Gray8:
                            {
                                for (int i = 0; i < frameStorage.FrameData[depth].Length; i++)
                                {
                                    sum += frameStorage.FrameData[depth][i];
                                    squareSum += frameStorage.FrameData[depth][i] * frameStorage.FrameData[depth][i];
                                }

                                double mean = sum / (frameStorage.FrameData[depth].Length / 2);
                                double stdev = Math.Sqrt((squareSum / (frameStorage.FrameData[depth].Length / 2)) - (mean * mean));

                                MinContrastValue = (int)Math.Max(0, (mean - 2 * stdev));
                                MaxContrastValue = (int)Math.Min(255, (mean + 2 * stdev));
                            }
                            break;
                    }
                    }
            }
        }


        public uint ViewportWidth
        {
            get => viewportWidth; set
            {
                viewportWidth = value;
                OnPropertyChanged();
            }
        }
        public uint ViewportHeight
        {
            get => viewportHeight; set
            {
                viewportHeight = value;
                OnPropertyChanged();
            }
        }

        protected override object DisplayOptions { get; } = new SkiaSharpDisplayOptions();

        protected override Type DisplayOptionsType { get; } = typeof(SkiaSharpDisplayOptions);

        public SkiaSharpDisplay(double framerate = 60) : base(framerate) { }
        protected override void CanvasDrawingFinalizer(SKCanvas canvas)
        {
            canvas.Flush();
        }
        protected override void DrawEmptyCanvas(SKCanvas canvas)
        {
            canvas.Clear(SKColors.Blue);
        }

        public static Vessel<SkiaSharpDisplay> GetVessel(IEnumerable<Vessel>? vesselsToDisplay = null, double framerate = 60)
        {
            Vessel<SkiaSharpDisplay> vessel = GetVessel(() => new SkiaSharpDisplay(framerate), vesselsToDisplay, framerate);
            return vessel;
        }
    }
}