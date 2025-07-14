using Sardine.Core.Views.WPF;
using Sardine.Devices.NI.DAQ.Controllers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sardine.Devices.NI.Views.WPF
{
    /// <summary>
    /// Interaction logic for SingleBitDigitalWriter_UI.xaml
    /// </summary>
    public partial class SingleBitDigitalWriter_UI : VesselUserControl<SimpleSingleBitDigitalWriter>
    {
        public SingleBitDigitalWriter_UI()
        {
            InitializeComponent();
        }
    }
}
