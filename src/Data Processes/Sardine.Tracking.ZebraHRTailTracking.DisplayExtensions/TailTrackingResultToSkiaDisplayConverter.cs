using Sardine.Display;
using SkiaSharp;

namespace Sardine.Tracking.ZebraHRTailTracking
{
    public class TailTrackingResultToSkiaDisplayConverter : IDrawableConverter<SKCanvas, TailTrackingResult>
    {
        public IDrawable<SKCanvas> Convert(TailTrackingResult drawable)
        {
            return new TailTrackingTrace(drawable);
        }

        public class TailTrackingTrace : IDrawable<SKCanvas>
        {
            public bool IsAlive => true;
            readonly SKPoint[] points = new SKPoint[16];
            readonly SKPaint paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Red,
                StrokeWidth = 5,
                StrokeCap = SKStrokeCap.Round
            };
            TailTrackingResult result;
            public TailTrackingTrace(TailTrackingResult result)
            {
                this.result = result;
            }
            readonly SKPointMode pointMode = SKPointMode.Points;

            public void Draw(SKCanvas canvas, ref object? displayData, uint canvasWidth, uint canvasHeight, object displayOptions, Type displayOptionsType)
            {
                float scale = Math.Min((float)canvasWidth / result.imageWidth, (float)canvasHeight / result.imageHeight);
                for (int i = 0; i < 16; i++)
                {
                    points[i].X = result.tailPoints[i].X * scale;
                    points[i].Y = result.tailPoints[i].Y * scale;
                }
                canvas.DrawPoints(pointMode, points, paint);
            }
        }
    }
}
