using Sardine.Core;
using Sardine.Core.Views.WPF;
using Sardine.Devices.Hamamatsu.Camera.API;

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Sardine.Devices.Hamamatsu.Camera.Views.WPF
{
    /// <summary>
    /// Interaction logic for CameraOptionsUI.xaml
    /// </summary>
    public partial class RemoteFramerateCameraOptionsUI : VesselUserControl<HamamatsuCamera>
    {
        public RemoteFramerateCameraOptionsUI()
        {
            InitializeComponent();
            ComboBox_Binning.ItemsSource = Enum.GetValues(typeof(SubarraySize)).Cast<SubarraySize>();
            ComboBox_CaptureSpeed.ItemsSource = Enum.GetValues(typeof(ReadoutSpeed)).Cast<ReadoutSpeed>();
        }

        private void Button_UpdateRegion_Click(object sender, RoutedEventArgs e)
        {
            if (IsOnline && Handle!.IsCapturing)
            {
                Handle?.StopCapture();
                Handle?.UpdateCaptureRegion();
                Handle?.PrepareCapture();
                Handle?.StartCapture();
                return;
            }

            Handle?.UpdateCaptureRegion();
        }

        private void ComboBox_Binning_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox_Binning.SelectedItem = Handle?.Binning;
        }

        private void Button_StartStop_Click(object sender, RoutedEventArgs e)
        {
            if (IsOnline && Handle!.IsCapturing)
            {
                Handle?.StopCapture();
                return;
            }

            Handle?.PrepareCapture();
            Handle?.StartCapture();
        }
    }
}
