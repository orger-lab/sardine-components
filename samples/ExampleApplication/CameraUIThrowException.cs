using ExampleSystem;
using Sardine.Core.Views.WPF;
using System.Windows;

namespace ExampleApplication
{
    /// <summary>
    /// Interaction logic for CameraBreakingUI.xaml
    /// </summary>
    public partial class CameraUIThrowException : VesselUserControl<Camera>
    {
        public CameraUIThrowException()
        {
            InitializeComponent();
        }

        private void Button_Break_Click(object sender, RoutedEventArgs e)
        {
            Handle?.BreakCamera();
        }
    }
}
