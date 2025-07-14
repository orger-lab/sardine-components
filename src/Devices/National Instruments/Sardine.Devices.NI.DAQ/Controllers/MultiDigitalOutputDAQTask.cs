using NationalInstruments.DAQmx;
using System.Collections.Generic;

namespace Sardine.Devices.NI.DAQ.Controllers
{
    public class MultiDigitalOutputDAQTask : DAQDevice
    {
        public DigitalMultiChannelWriter DigitalChannelWriter { get; }

        public MultiDigitalOutputDAQTask(DAQBoard board, string name,
                                         IEnumerable<DaqPhysicalChannelID> digitalOutChannels)
                                  : base(board, name, null, digitalOutChannels ?? [])
        {
            DigitalChannelWriter = new DigitalMultiChannelWriter(MainTask.Stream);
        }

        public override void SetupChannels()
        {
            foreach (var channel in MainTask.Channels)
            {
                MainTask.DOChannels.CreateChannel(channel.ToString(), $"{MainTask.Name}_{channel.FriendlyName}_Out", ChannelLineGrouping.OneChannelForEachLine);
            }
        }
    }

}
