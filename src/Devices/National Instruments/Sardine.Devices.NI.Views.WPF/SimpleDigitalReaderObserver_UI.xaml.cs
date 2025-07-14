using Sardine.Core.Views.WPF;
using Sardine.Devices.NI.DAQ.Controllers;

namespace Sardine.Devices.NI.Views.WPF
{
    /// <summary>
    /// Interaction logic for SimpleDigitalReaderObserver_UI.xaml
    /// </summary>
    public partial class SimpleDigitalReaderObserver_UI : VesselUserControl<SimpleDITriggerReader>
    {
        public SimpleDigitalReaderObserver_UI()
        {
            InitializeComponent();
        }
    }
}
