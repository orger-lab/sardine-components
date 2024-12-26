using ExampleSystem;
using Sardine.Core.Views.WPF;
using System.Windows;

namespace ExampleApplication
{
    /// <summary>
    /// Interaction logic for CameraUI.xaml
    /// </summary>
    public partial class CameraUI : VesselUserControl<Camera>
    {
        public CameraUI()
        {
            InitializeComponent();
        }
        private void Button_StartStop_Click(object sender, RoutedEventArgs e)
        {
            if (Handle is not null)
            {
                if (Handle.IsRunning)
                    Handle?.Stop();
                else
                    Handle?.Start();
            }
        }
    }

}
