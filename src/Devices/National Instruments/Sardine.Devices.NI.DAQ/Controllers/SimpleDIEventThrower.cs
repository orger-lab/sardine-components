using NationalInstruments.DAQmx;
using System;

namespace Sardine.Devices.NI.DAQ.Controllers
{
    public sealed class SimpleDIEventThrower : SimpleDAQReader<byte>
    {
        private readonly DigitalSingleChannelReader singleReadoutReader;

        public SimpleDIEventThrower(DAQBoard board, DaqPhysicalChannelID channelID, string name) : base(board, name,null, channelID)
        {
            if (channelID.ChannelType != PhysicalChannelTypes.DILine)
                throw new ArgumentOutOfRangeException(nameof(channelID));

            singleReadoutReader = new DigitalSingleChannelReader(MainTask.Stream);
        }


        void OnDigitalChangeDetection(object sender, DigitalChangeDetectionEventArgs e)
        {
            Read();
        }

        protected override byte GetReading() => (byte)(singleReadoutReader.ReadSingleSampleSingleLine() ? 1 : 0);

        public bool ReadBool() => Read() == 1;
        public override void SetupChannels()
        {
            MainTask.DIChannels.CreateChannel(MainTask.Channels[0].ToString(), $"{MainTask.Name}_{MainTask.Channels[0].FriendlyName}", ChannelLineGrouping.OneChannelForEachLine);

            MainTask.Timing.ConfigureChangeDetection(
                                MainTask.Channels[0].ToString(),
                                MainTask.Channels[0].ToString(),
                                SampleQuantityMode.ContinuousSamples, 1000);

            MainTask.SynchronizeCallbacks = true;
            MainTask.DigitalChangeDetection += new DigitalChangeDetectionEventHandler(OnDigitalChangeDetection);
            MainTask.Reserve();
        }
    }
}
