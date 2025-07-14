using Sardine.Utils.Measurements;
using Sardine.Utils.Measurements.Time;

using System;

namespace Sardine.Devices.NI.DAQ
{
    public abstract class SimpleDAQReader<T> : DAQDevice where T : struct
    {
        private T _value;

        protected System.Timers.Timer? samplerTimer;

        public T Value
        {
            get => _value;
            protected set
            {
                if (!_value.Equals(value))
                {
                    _value = value;
                    ValueChanged?.Invoke(this, new DaqReaderEventEventArgs<T>(Value, MainTask.Name));
                }
            }
        }

        public event EventHandler<DaqReaderEventEventArgs<T>>? ValueChanged;
        public event EventHandler<DaqReaderEventEventArgs<T>>? OnNewSample;

        public SimpleDAQReader(DAQBoard board, string name, object? parameters, DaqPhysicalChannelID readerChannel) : base(board, name, parameters, readerChannel) { }

        protected abstract T GetReading();

        public void StopSampler()
        {
            samplerTimer?.Stop();
            MainTask.Unreserve();
        }

        public void StartSampler(Measure<Hertz> frequency)
        {
            if (!MainTask.ChannelsAvailable)
                throw new Exception();

            MainTask.Reserve();

            samplerTimer = new System.Timers.Timer
            {
                Interval = 1000 / frequency
            };
            samplerTimer.Elapsed += SamplerTimer_Elapsed;

            samplerTimer?.Start();
        }

        public T Read()
        {
            Value = GetReading();
            return Value;
        }

        void SamplerTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            Value = GetReading();
            OnNewSample?.Invoke(this, new DaqReaderEventEventArgs<T>(Value, MainTask.Name));
        }

        protected override void Dispose(bool disposing)
        {
            StopSampler();
            base.Dispose(disposing);
        }
    }
}
