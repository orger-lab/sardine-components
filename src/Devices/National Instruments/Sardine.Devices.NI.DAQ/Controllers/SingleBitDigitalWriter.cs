using System;

using NationalInstruments.DAQmx;


namespace Sardine.Devices.NI.DAQ.Controllers
{
    public sealed class SimpleSingleBitDigitalWriter : DAQDevice
    {

        private readonly DigitalSingleChannelWriter taskWriter;


        public bool? LastWrittenSample { get; private set; }

        public event EventHandler? OnSampleWritten;


        public SimpleSingleBitDigitalWriter(DAQBoard board, DaqPhysicalChannelID channelID, string name) : base(board,name,null,channelID)
        {
            if (channelID.ChannelType != PhysicalChannelTypes.DOLine)
                throw new ArgumentOutOfRangeException(nameof(channelID));


            taskWriter = new DigitalSingleChannelWriter(MainTask.Stream);

        }

        public override void SetupChannels()
        {
            MainTask.DOChannels.CreateChannel(MainTask.Channels[0].ToString(), $"{MainTask.Name}_{MainTask.Channels[0].FriendlyName}", ChannelLineGrouping.OneChannelForEachLine);
            MainTask.Reserve();
        }

        public void Write(bool valueToWrite)
        {

            taskWriter.WriteSingleSampleSingleLine(true, valueToWrite);
            LastWrittenSample = valueToWrite;

            OnSampleWritten?.Invoke(this, EventArgs.Empty);

        }


    }

}