using NationalInstruments.DAQmx;
using Sardine.Utils.Measurements;
using Sardine.Utils.Measurements.Electric;
using System.Collections.Generic;

namespace Sardine.Devices.NI.DAQ.Controllers
{
    public class MultiAnalogOutputDAQTask : DAQDevice
    {
        public AnalogMultiChannelWriter AnalogChannelWriter { get; }

        public MultiAnalogOutputDAQTask(DAQBoard board, string name,
                                        IEnumerable<DaqPhysicalChannelID> analogOutChannels,
                                        Measure<Volt> minValue, Measure<Volt> maxValue)
                                 : base(board, name, (minValue, maxValue), analogOutChannels ?? [])
        {
            AnalogChannelWriter = new AnalogMultiChannelWriter(MainTask.Stream);
        }

        public override void SetupChannels()
        {
            var parameters = ((Measure<Volt> MinValue, Measure<Volt> MaxValue))ChannelParameters!;

            foreach (var channel in MainTask.Channels)
            {
                if (channel.ChannelType == PhysicalChannelTypes.AO)
                    MainTask.AOChannels.CreateVoltageChannel(channel.ToString(), $"{MainTask.Name}_{channel.FriendlyName}_Out", parameters.MinValue, parameters.MaxValue, AOVoltageUnits.Volts);
            }
        }
    }

}
