using Sardine.Core.Views.WPF;
using Sardine.Devices.NI.DAQ.Controllers;

namespace Sardine.Devices.NI.Views.WPF
{
    /// <summary>
    /// Interaction logic for ClockSourceGenerator_UI.xaml
    /// </summary>
    public partial class ClockSourceGenerator_UI : VesselUserControl<ClockSource>
    {
        public ClockSourceGenerator_UI()
        {
            InitializeComponent();
        }

        private void Button_Arm_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Handle is not null)
            {
                if (Handle.ClockArmed)
                    Handle.Disarm();
                else
                    Handle.Arm();
            }
        }

        private void Button_Start_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Handle is not null)
            {
                if (Handle.ClockRunning)
                    Handle.Stop();
                else
                    Handle.Start();
            }
        }
    }
}
