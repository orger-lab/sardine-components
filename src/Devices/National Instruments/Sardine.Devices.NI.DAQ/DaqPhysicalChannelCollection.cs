using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sardine.Devices.NI.DAQ
{
    public sealed class DaqPhysicalChannelCollection : IEnumerable<DaqPhysicalChannelID>
    {
        public IReadOnlyList<DaqPhysicalChannelID> Channels { get; } = new List<DaqPhysicalChannelID>();


        public DaqPhysicalChannelID this[int id]
        {
            get => Channels[id];
        }

        public event EventHandler? DaqPhysicalCollectionReservedStatusChanged;

        public DaqPhysicalChannelCollection(IEnumerable<DaqPhysicalChannelID> channels)
        {
            var channelList = new List<DaqPhysicalChannelID>();

            foreach (DaqPhysicalChannelID channel in channels)
            {
                channelList.Add(channel);
                channel.OnReservedStatusChanged += Channel_OnReservedStatusChanged;
            }

            Channels = channelList;
        }

        public bool AreChannelsAvailable(DAQTask task) => !Channels.Any(x => x.IsReserved && x.ReservedBy != task);

        private void Channel_OnReservedStatusChanged(object? sender, EventArgs e)
        {
            DaqPhysicalCollectionReservedStatusChanged?.Invoke(this, EventArgs.Empty);
        }

        internal DaqPhysicalChannelCollection() { }

        public static DaqPhysicalChannelCollection GenerateChannelCollection(object? caller)
        {
            List<DaqPhysicalChannelID> list = [];

            if (caller is not null)

                foreach (PropertyInfo property in caller.GetType().GetProperties()
                    .Where((prop) => prop.PropertyType == typeof(DaqPhysicalChannelID)))
                {
                    DaqPhysicalChannelID? channel = (DaqPhysicalChannelID?)property.GetValue(caller);
                    if (channel is not null)
                    {
                        channel.FriendlyName = property.Name;
                        list.Add(channel);
                    }
                }


            return new DaqPhysicalChannelCollection(list);

        }        

        public IEnumerator<DaqPhysicalChannelID> GetEnumerator() => Channels.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
