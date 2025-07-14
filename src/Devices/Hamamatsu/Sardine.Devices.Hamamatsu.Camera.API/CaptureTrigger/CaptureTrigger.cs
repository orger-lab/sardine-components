using System;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    public class CaptureTrigger
    {
        public TriggerSource TriggerSource { get; internal set; }
        public TriggerPolarity TriggerPolarity { get; internal set; }
        public TriggerMode TriggerMode { get; internal set; }
        public TriggerActive TriggerActive { get; internal set; }
        public TriggerGlobalExposure TriggerGlobalExposure { get; internal set; }
        public int TriggerTimes { get; internal set; }
        public double TriggerDelay { get; internal set; }

        private Action? FireAction { get; set; } = null;

        internal CaptureTrigger() { }

        public void Fire()
        {
            FireAction?.Invoke();
        }

        public void Set(DCam api)
        {
            if (api.PropertyCollection.TriggerSource == null) throw new InvalidOperationException();
            if (api.PropertyCollection.TriggerPolarity == null) throw new InvalidOperationException();
            if (api.PropertyCollection.TriggerMode == null) throw new InvalidOperationException();
            if (api.PropertyCollection.TriggerActive == null) throw new InvalidOperationException();
            if (api.PropertyCollection.TriggerGlobalExposure == null) throw new InvalidOperationException();
            if (api.PropertyCollection.TriggerTimes == null) throw new InvalidOperationException();
            if (api.PropertyCollection.TriggerDelay == null) throw new InvalidOperationException();

            api.PropertyCollection.TriggerSource.Value = TriggerSource;
            api.PropertyCollection.TriggerPolarity.Value = TriggerPolarity;
            api.PropertyCollection.TriggerMode.Value = TriggerMode;
            api.PropertyCollection.TriggerActive.Value = TriggerActive;
            api.PropertyCollection.TriggerGlobalExposure.Value = TriggerGlobalExposure;
            api.PropertyCollection.TriggerTimes.Value = TriggerTimes;
            api.PropertyCollection.TriggerDelay.Value = TriggerDelay;
        }

        public static CaptureTrigger Get(DCam api)
        {
            TriggerSource source = api.PropertyCollection.TriggerSource?.Value ?? 0;
            return new CaptureTrigger()
            {
                TriggerSource = source,
                TriggerPolarity = api.PropertyCollection.TriggerPolarity?.Value ?? 0,
                TriggerDelay = api.PropertyCollection.TriggerDelay?.Value ?? 0,
                TriggerTimes = api.PropertyCollection.TriggerTimes?.Value ?? 0,
                TriggerActive = api.PropertyCollection.TriggerActive?.Value ?? 0,
                TriggerGlobalExposure = api.PropertyCollection.TriggerGlobalExposure?.Value ?? 0,
                TriggerMode = api.PropertyCollection.TriggerMode?.Value ?? 0,
                FireAction = source == TriggerSource.Software ? () => api.FireTrigger() : null,
            };
        }
    }
}
