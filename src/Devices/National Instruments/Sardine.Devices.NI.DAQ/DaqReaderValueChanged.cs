using System;

namespace Sardine.Devices.NI.DAQ
{
    public sealed class DaqReaderEventEventArgs<T> : EventArgs
    {
        public T NewValue { get; private set; }
        public string DeviceName { get; private set; }

        public DaqReaderEventEventArgs(T newValue, string deviceName)
        {
            NewValue = newValue;
            DeviceName = deviceName;
        }
    }
}
