using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Sardine.Core.Views.WPF;
using Sardine.Devices.Hamamatsu.Camera.API;
using Sardine.Utils.Windows;

namespace Sardine.Devices.Hamamatsu.Camera.Views.WPF
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class CameraInfoUI : VesselUserControl<IHamamatsuAPIWrapper>
    {
        private bool isFetching;

        public bool IsFetching
        {
            get => isFetching; set
            {
                if (isFetching != value)
                {
                    isFetching = value;
                    switch (IsFetching)
                    {
                        case true:
                            Mouse.OverrideCursor = Cursors.AppStarting;
                            break;
                        case false:
                            Mouse.OverrideCursor = null;
                            break;
                    }
                    OnPropertyChanged();
                }
            }
        }

        public CameraInfoUI()
        {
            InitializeComponent();
        }

        public override void OnVesselReloadedAction()
        {
            Dispatcher.BeginInvoke(() => UpdateCameraListBox(reset: false));
        }

        private void Button_Fetch_Click(object sender, RoutedEventArgs e)
        {
            new Task(() => UpdateCameraListBox(reset: true)).Start();
            
        }

        static void ResetCameras()
        {
            DisableHardware.ResetDevice((x) => x.Contains("VID_0661"));
            DisableHardware.ResetDevice((x) => x.Contains("FB014153"));
        }

        void UpdateCameraListBox(bool reset)
        {
            if (Handle is null)
                return;

            Task resetTask = new(() =>
            {
                ResetCameras();
                HamamatsuCamera.FetchCameraMetadata(Handle);
            });

            if (reset)
            {
                resetTask.Start();
            }

                Dispatcher.BeginInvoke(() =>
            {
                IsFetching = true;
                
                if (reset)
                    resetTask.Wait();

                ListBoxItem[] items = new ListBoxItem[HamamatsuCamera.CameraMetadata.Length];
                for (int i = 0; i < items.Length; i++)
                {
                    items[i] = new ListBoxItem
                    {
                        Content = $"{HamamatsuCamera.CameraMetadata[i].CameraID}: {HamamatsuCamera.CameraMetadata[i].Model} [{HamamatsuCamera.CameraMetadata[i].Bus}]",
                        Focusable = false
                    };
                }

                ListBox_Lines.ItemsSource = items;

                IsFetching = false;
            });
        }
    }
}