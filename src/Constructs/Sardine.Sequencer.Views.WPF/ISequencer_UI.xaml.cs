using Sardine.Core.Views.WPF;
using Sardine.Utils.Measurements.Time;
using Sardine.Utils.Measurements;
using System.ComponentModel;
using System.Windows;

namespace Sardine.Sequencer.Views.WPF
{
    /// <summary>
    /// Interaction logic for DAQSequencer_UI.xaml
    /// </summary>
    public partial class ISequencer_UI : VesselUserControl<ISequencer>
    {
        private int sequenceLength;

        public int NumberOfPatterns
        {
            get => sequenceLength; set
            {
                sequenceLength = value;
                OnPropertyChanged();
            }
        }

        public ISequencer_UI()
        {
            InitializeComponent();

            PropertyChanged += PropertyChangedEventCallback;
        }

        void PropertyChangedEventCallback(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentPattern")
            {
                OnPropertyChanged("ExecutionProgressPercentage");
                OnPropertyChanged("ExpectedTimeLeft");
            }
        }
    public double ExecutionProgressPercentage => (Handle is null)?0:(Handle.TotalSequenceLength == 0 ? 0 : Math.Min(100, 100 * Handle.CurrentPattern / Handle.TotalSequenceLength));
    public Measure<Second> ExpectedTimeLeft => (Handle is null)?0:(Handle.ExecutionRate == 0 ? 0 : Math.Max(0, (Handle.TotalSequenceLength - Handle.CurrentPattern) / Handle.ExecutionRate));


    private void Button_Start_Click(object sender, RoutedEventArgs e)
        {
            if (Handle is not null && Handle.IsArmed)
            {
                if (Handle.IsRunning)
                {
                    Handle.Stop();
                }
                else
                {
                    Handle.Start(NumberOfPatterns);
                }
            }
        }
    }
}
    
