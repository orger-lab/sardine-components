using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    public class CaptureTriggerBuilder : INotifyPropertyChanged
    {
        private TriggerSource triggerSource;
        private TriggerPolarity triggerPolarity;
        private TriggerMode triggerMode;
        private TriggerActive triggerActive;
        private TriggerGlobalExposure triggerGlobalExposure;
        private int triggerTimes;
        private double triggerDelay;

        public TriggerSource TriggerSource { get => triggerSource; set { triggerSource = value; OnPropertyChanged(); } }
        public TriggerPolarity TriggerPolarity { get => triggerPolarity; set { triggerPolarity = value; OnPropertyChanged(); } }
        public TriggerMode TriggerMode { get => triggerMode; set { triggerMode = value; OnPropertyChanged(); } }
        public TriggerActive TriggerActive { get => triggerActive; set { triggerActive = value; OnPropertyChanged(); } }
        public TriggerGlobalExposure TriggerGlobalExposure { get => triggerGlobalExposure; set { triggerGlobalExposure = value; OnPropertyChanged(); } }
        public int TriggerTimes { get => triggerTimes; set { triggerTimes = value; OnPropertyChanged(); } }
        public double TriggerDelay { get => triggerDelay; set { triggerDelay = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler? PropertyChanged;

        public CaptureTrigger Build()
        {
            return new CaptureTrigger
            {
                TriggerSource = TriggerSource,
                TriggerPolarity = TriggerPolarity,
                TriggerMode = TriggerMode,
                TriggerActive = TriggerActive,
                TriggerGlobalExposure = TriggerGlobalExposure,
                TriggerTimes = TriggerTimes,
                TriggerDelay = TriggerDelay,
            };
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
