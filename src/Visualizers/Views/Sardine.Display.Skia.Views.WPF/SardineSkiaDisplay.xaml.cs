using Sardine.Core.Views.WPF;
using System.Windows.Controls;
using System.Windows.Media;

namespace Sardine.Display.Skia.Views.WPF
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SardineSkiaDisplay : VesselUserControl<SkiaSharpDisplay>
    {
        public SardineSkiaDisplay()
        {
            InitializeComponent();
        }

        private void SardineSkiaDisplay_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Handle is not null) Handle.OnNewFrameReady -= SardineSkiaDisplay_OnNewFrameReady; 
        }

        public override void OnVesselReloadedAction()
        {
            if (Handle is not null)
            {
                Handle!.OnNewFrameReady += SardineSkiaDisplay_OnNewFrameReady;
            }
        }

        private void SardineSkiaDisplay_OnNewFrameReady(object sender, System.EventArgs e)
        {
            if (Dispatcher.HasShutdownStarted)
            {
                if (Handle is not null)
                    Handle.OnNewFrameReady -= SardineSkiaDisplay_OnNewFrameReady;
                return;
            }


            Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    SKElement_Display.InvalidateVisual();
                }
                catch { }
            });
        }

        private void SKElement_Display_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            Handle?.PaintCanvas(e.Surface.Canvas, (uint)e.Info.Width, (uint)e.Info.Height);

        }

        private void Button_SetContrast_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Handle?.SetContrast();
        }

        private void SKElement_Display_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var dpiScale = VisualTreeHelper.GetDpi(this);

            var dpiScaleX = dpiScale.DpiScaleX;
            var dpiScaleY = dpiScale.DpiScaleY;

            var pixelPosition = e.GetPosition(sender as Canvas);
            var scaledPixelPosition = new System.Windows.Point(pixelPosition.X * dpiScaleX, pixelPosition.Y * dpiScaleY);

            // Console.WriteLine($"{pixelPosition.X},{pixelPosition.Y} {scaledPixelPosition.X}{scaledPixelPosition.Y}");
        }
    }
}