using NationalInstruments.DAQmx;

using System;

namespace Sardine.Devices.NI.DAQ.Controllers
{
    public sealed class SimpleDigitalEdgeCounter : SimpleDAQReader<int>
    {
        private readonly CounterSingleChannelReader singleReadoutReader;

        public SimpleDigitalEdgeCounter(DAQBoard board,
                                        DaqPhysicalChannelID channelID,
                                        string name,
                                        CICountEdgesActiveEdge activeEdge = CICountEdgesActiveEdge.Rising,
                                        long initialCount = 0,
                                        CICountEdgesCountDirection countDirection = CICountEdgesCountDirection.Up) : base(board,  name, (activeEdge, initialCount, countDirection), channelID)
        {
            if (channelID.ChannelType != PhysicalChannelTypes.CI)
                throw new ArgumentOutOfRangeException(nameof(channelID));

            singleReadoutReader = new CounterSingleChannelReader(MainTask.Stream);
        }

        protected override int GetReading() => singleReadoutReader.ReadSingleSampleInt32();
        public override void SetupChannels()
        {
            (CICountEdgesActiveEdge activeEdge, long initialCount, CICountEdgesCountDirection countDirection) parameters = ((CICountEdgesActiveEdge, long, CICountEdgesCountDirection))ChannelParameters!;

            MainTask.CIChannels.CreateCountEdgesChannel(MainTask.Channels[0].ToString(), $"{MainTask.Name}_{MainTask.Channels[0].FriendlyName}", parameters.activeEdge, parameters.initialCount, parameters.countDirection);
        }
    }

}
