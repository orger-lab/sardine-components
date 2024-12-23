using Sardine.ImageProcessing;
using SkiaSharp;

using System.Runtime.InteropServices;

namespace Sardine.Display.Skia
{

    public sealed class ImageFrameSkia : IDrawable<SKCanvas>, IImageFrame
    {
        private IImageFrame ImageFrame { get; }

        public IReadOnlyList<byte[]>? FrameData => ImageFrame.FrameData;

        public PixelFormat PixelFormat => ImageFrame.PixelFormat;


        public double UnitXSize => ImageFrame.UnitXSize;

        public double UnitYSize => ImageFrame.UnitYSize;

        public double UnitZSize => ImageFrame.UnitZSize;

        public int Width => ImageFrame.Width;

        public int Height => ImageFrame.Height;

        public bool IsAlive => ImageFrame.IsAlive;

        public int Planes => ImageFrame.Planes;

        public string CameraSN => ImageFrame.CameraSN;

        public string CameraModel => ImageFrame.CameraModel;

        public int FrameID => ImageFrame.FrameID;

        public IReadOnlyList<DateTime> Timestamps => ImageFrame.Timestamps;

        public ImageFrameSkia(IImageFrame frame)
        {
            ImageFrame = frame;
        }




        public void Draw(SKCanvas canvas, ref object? displayData, uint canvasWidth, uint canvasHeight, object displayOptions, Type displayOptionsType)
        {
            if (IsAlive && ImageFrame.FrameData is not null)
            {
                switch (ImageFrame.PixelFormat)
                {
                    case PixelFormat.Gray16:
                        // TODO: Update this method
                        displayData = DrawGray16(canvas, displayData, canvasWidth, canvasHeight, displayOptions);
                        break;
                    case PixelFormat.Gray8:
                        DrawGray8(canvas, canvasWidth, canvasHeight, displayOptions);
                        break;
                    default:
                        return;
                }
            }
            if (canvasWidth > 100 && canvasHeight > 50)
            {
                canvas.DrawText($"{FrameID}", 10, 20, writingPaint);
                canvas.DrawText($"{Timestamps[0].Hour:00}:{Timestamps[0].Minute:00}:{Timestamps[0].Second:00}.{Timestamps[0].Millisecond}", 10, 40, writingPaint);
            }

        }

        static SKPaint writingPaint = new SKPaint() { TextSize = 20, Color = SKColors.DarkRed };

        private void DrawGray8(SKCanvas canvas, uint canvasWidth, uint canvasHeight, object displayOptions)
        {
                SKImageInfo info = new(ImageFrame.Width, ImageFrame.Height, SKColorType.Gray8, SKAlphaType.Opaque);
                var gcHandle = GCHandle.Alloc(ImageFrame.FrameData![0], GCHandleType.Pinned);
                SKImage image = SKImage.FromPixels(info, gcHandle.AddrOfPinnedObject(), info.RowBytes);

                if (ImageFrame.Width > ImageFrame.Height)
                        info = new((int)canvasWidth, (int)(canvasHeight * ImageFrame.Height / ImageFrame.Width), SKColorType.Gray8, SKAlphaType.Opaque);
                    else
                        info = new((int)(canvasWidth * ImageFrame.Width / ImageFrame.Height), (int)canvasHeight, SKColorType.Gray8, SKAlphaType.Opaque);

                        canvas.DrawImage(image, new SKRect(0,0, info.Width, info.Height));


                image.Dispose();
                gcHandle.Free();
        }

        private object DrawGray16(SKCanvas canvas, object? displayData, uint canvasWidth, uint canvasHeight, object displayOptions)
        {
            using (SKBitmap bitmap = new())
            {
                if (displayData is not ImageFrameSkiaStorage || ((ImageFrameSkiaStorage)displayData).StorageSize != ImageFrame.Width*ImageFrame.Height)
                    displayData = new ImageFrameSkiaStorage(ImageFrame.Width * ImageFrame.Height);

                if (ImageFrame.Planes <= ((SkiaSharpDisplayOptions)displayOptions).DepthToShow)
                    ((SkiaSharpDisplayOptions)displayOptions).DepthToShow = 0;

                if (((SkiaSharpDisplayOptions)displayOptions).AutoContrast)
                {
                    int randomPoint = DrawingExtensions.From16BitArray(ImageFrame.FrameData![((SkiaSharpDisplayOptions)displayOptions).DepthToShow],2*(int)(0.3 * ImageFrame.Width * ImageFrame.Height));
                    ((SkiaSharpDisplayOptions)displayOptions).MinContrastValue = (DrawingExtensions.FindMinimaGray16(ImageFrame.FrameData![((SkiaSharpDisplayOptions)displayOptions).DepthToShow]) + randomPoint) / 2;
                    ((SkiaSharpDisplayOptions)displayOptions).MaxContrastValue = (DrawingExtensions.FindMaximaGray16(ImageFrame.FrameData![((SkiaSharpDisplayOptions)displayOptions).DepthToShow]) + randomPoint) / 2;
                }


                DrawingExtensions.Gray16ToGray8Compressor(ImageFrame.FrameData![((SkiaSharpDisplayOptions)displayOptions).DepthToShow], ref ((ImageFrameSkiaStorage)displayData).Data,
                                                          ((SkiaSharpDisplayOptions)displayOptions).MinContrastValue, ((SkiaSharpDisplayOptions)displayOptions).MaxContrastValue);


                SKImageInfo info = new(ImageFrame.Width, ImageFrame.Height, SKColorType.Gray8, SKAlphaType.Opaque);
                bitmap.InstallPixels(info,
                                     ((ImageFrameSkiaStorage)displayData).Handle.AddrOfPinnedObject(), info.RowBytes, delegate
                                     {
 
                                     }, null);
                
                if (ImageFrame.Width != canvasWidth || ImageFrame.Height != canvasHeight)
                {
                    SKImageInfo resizedInfo;
                    if (ImageFrame.Width > ImageFrame.Height)
                        resizedInfo = new((int)canvasWidth, (int)(canvasHeight * ImageFrame.Height / ImageFrame.Width), SKColorType.Gray8, SKAlphaType.Opaque);
                    else
                        resizedInfo = new((int)(canvasWidth * ImageFrame.Width / ImageFrame.Height), (int)canvasHeight, SKColorType.Gray8, SKAlphaType.Opaque);


                    using (SKBitmap resizedBitmap = bitmap.Resize(resizedInfo, SKFilterQuality.None))
                    {
                        canvas.DrawBitmap(resizedBitmap, 0, 0);

                        resizedBitmap.Dispose();
                    }


                }
                else
                    canvas.DrawBitmap(bitmap, 0, 0);
            }

            return displayData;
        }

        public partial class SKCanvasDrawableConverter : IDrawableConverter<SKCanvas, IImageFrame>
        {
            public IDrawable<SKCanvas> Convert(IImageFrame image) => new ImageFrameSkia(image);
        }
    }
}
