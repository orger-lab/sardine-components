using Sardine.Core.Views.WPF;
using Sardine.Devices.NI.DAQ;

namespace Sardine.Devices.NI.Views.WPF
{
    /// <summary>
    /// Interaction logic for DaqInfoUI.xaml
    /// </summary>
    public partial class DaqInfoUI : VesselUserControl<DAQBoard>
    {
        public DaqInfoUI()
        {
            InitializeComponent();                
        }

        public override void OnVesselReloadedAction()
        {
            
            Handle!.OnTaskCollectionChanged += (_,_) =>
            Dispatcher.Invoke(() => ListBox_Channels.Items.Refresh());
        }
    }
}
