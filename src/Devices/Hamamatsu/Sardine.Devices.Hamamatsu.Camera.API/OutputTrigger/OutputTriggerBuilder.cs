using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    public class OutputTriggerBuilder : INotifyPropertyChanged
    {
        private OutputTriggerSource source;
        private OutputTriggerPolarity polarity;
        private OutputTriggerActiveRegion activeRegion;
        private OutputTriggerKind kind;
        private double delay;
        private double period;

        public OutputTriggerSource Source { get => source; set { source = value; OnPropertyChanged(); } }
        public OutputTriggerPolarity Polarity { get => polarity; set { polarity = value; OnPropertyChanged(); } }
        public OutputTriggerActiveRegion ActiveRegion { get => activeRegion; set { activeRegion = value; OnPropertyChanged(); } }
        public OutputTriggerKind Kind { get => kind; set { kind = value; OnPropertyChanged(); } }
        public double Delay { get => delay; set { delay = value; OnPropertyChanged(); } }
        public double Period { get => period; set { period = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler? PropertyChanged;

        public OutputTrigger Build()
        {
            return new OutputTrigger
            { Source = Source, Polarity = Polarity, ActiveRegion = ActiveRegion, Kind = Kind, Delay = Delay, Period = Period };
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
