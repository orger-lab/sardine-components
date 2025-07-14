using NationalInstruments.DAQmx;

using System;
using System.Collections.Generic;

namespace Sardine.Devices.NI.DAQ
{
    public sealed class DaqPhysicalChannelID : IDAQReadable
    {
        private DAQTask? reservedBy;

        public PhysicalChannelTypes ChannelType { get; }
        public PhysicalChannelAccess ChannelAccess { get; }
        public string ChannelName { get; }

        public string FriendlyName { get; set; }

        public bool IsReserved => ReservedBy is not null;

        public DAQTask? ReservedBy
        {
            get => reservedBy; internal set
            {
                reservedBy = value;
                OnReservedStatusChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler? OnReservedStatusChanged;

        public DaqPhysicalChannelID(string name, PhysicalChannelTypes type, PhysicalChannelAccess access, string? friendlyName = null)
        {
            ChannelName = name;
            ChannelType = type;
            ChannelAccess = access;
            FriendlyName = friendlyName ?? name.Replace('/','_');
        }

        public override string ToString() => ChannelName;
        public IReadOnlyList<DaqPhysicalChannelID> GetChannels() => [this];
    }
}
