using Sardine.Core.Views.WPF;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sardine.ImageProcessing.BGSubtraction.OpenCV.Views.WPF
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class BackgroundSubtracterOpenCV_UI : VesselUserControl<OpenCVBackgroundSubtracter>
    {
        public BackgroundSubtracterOpenCV_UI()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Handle?.UpdateBackground();
        }

        private void Button_ShowBackground_Click(object sender, RoutedEventArgs e)
        {
            if (Handle?.BackgroundModel is null)
                return;

            var x = new ImageWindow();
            BitmapSource bitmapSource = BitmapSource.Create(Handle!.BackgroundModel!.Width, Handle!.BackgroundModel!.Height, 300, 300, PixelFormats.Indexed8, BitmapPalettes.Gray256, Handle?.BackgroundModel.FrameData[0], Handle!.BackgroundModel!.Width);
            x.Image.Source = bitmapSource;

            x.Show();
        }
    }

}
