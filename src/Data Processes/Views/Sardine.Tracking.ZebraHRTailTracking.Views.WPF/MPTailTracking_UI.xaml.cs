using Sardine.Core.Views.WPF;

using System.Windows.Controls;
using System.Windows.Input;

namespace Sardine.Tracking.ZebraHRTailTracking.Views.WPF
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MPTailTracking_UI : VesselUserControl<MPTailTracking>
    {
        public MPTailTracking_UI()
        {
            InitializeComponent();
        }

        private void TextBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ((TextBox)sender).Text = $"{Convert.ToInt32(((TextBox)sender).Text) + (e.Delta / 120.0)}";
        }
    }

}
