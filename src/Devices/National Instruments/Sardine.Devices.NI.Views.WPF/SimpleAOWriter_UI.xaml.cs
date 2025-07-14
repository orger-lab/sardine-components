using Sardine.Core.Views.WPF;
using Sardine.Devices.NI.DAQ.Controllers;
using Sardine.Utils.Measurements.Electric;
using Sardine.Utils.Measurements;
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
using System.ComponentModel;

namespace Sardine.Devices.NI.Views.WPF
{
    /// <summary>
    /// Interaction logic for SimpleAOWriter_UI.xaml
    /// </summary>
    public partial class SimpleAOWriter_UI : VesselUserControl<SimpleAOWriter>, INotifyPropertyChanged
    {
        private Measure<Volt> _value = 0;

        public Measure<Volt> Value
        {
            get => _value; set
            {
                _value = value;
                OnPropertyChanged();
            }
        }


        public SimpleAOWriter_UI()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Handle?.WriteValue(Value);
        }


    }
}
