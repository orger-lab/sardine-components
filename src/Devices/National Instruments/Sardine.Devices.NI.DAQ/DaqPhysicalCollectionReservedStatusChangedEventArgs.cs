using System;

namespace Sardine.Devices.NI.DAQ
{
    public class DaqPhysicalCollectionReservedStatusChangedEventArgs : EventArgs
    {
        public bool AreChannelsAvailable { get; }

        public DaqPhysicalCollectionReservedStatusChangedEventArgs(bool areChannelsAvailable)
        {
            AreChannelsAvailable = areChannelsAvailable;
        }
    }
}
