using Sardine.Core;
using Sardine.Core.Views.WPF;
using Sardine.Devices.Hamamatsu.Camera.API;


using System.Windows;

namespace Sardine.Devices.Hamamatsu.Camera.Views.WPF
{
    /// <summary>
    /// Interaction logic for CameraStatusUI.xaml
    /// </summary>
    public partial class CameraStatusUI : VesselUserControl<HamamatsuCamera>
    {
        public override void OnVesselReloadedAction()
        {
            Dispatcher.BeginInvoke(()=> { Handle!.OnNewError += Handle_OnNewError; });
        }

        private void Handle_OnNewError(object sender, OnDCamErrorEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                Label_ErrorMessage.Content = Handle?.LastInternalError;
            });
        }

        public CameraStatusUI()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (IsOnline)
            {
                var k = new CameraOptionsUI();
                k.LinkedVesselName = Vessel!.Name;

                Window window = new()
                {
                    Title = Title,
                    Content = k,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    ResizeMode = ResizeMode.NoResize
                };
                Unloaded += (_, _) => window.Close();
                window.Show();

            }
        }
    }
}
