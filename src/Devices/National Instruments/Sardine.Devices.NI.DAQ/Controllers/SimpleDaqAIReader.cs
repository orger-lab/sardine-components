using NationalInstruments.DAQmx;


namespace Sardine.Devices.NI.DAQ.Controllers
{
    public sealed class SimpleDaqAIReader : SimpleDAQReader<double>
    {
        private readonly AnalogSingleChannelReader analogReader;

        public SimpleDaqAIReader(DAQBoard board, DaqPhysicalChannelID channelID, double minVoltage, double maxVoltage, string name, AITerminalConfiguration terminalConfiguration = AITerminalConfiguration.Rse) : base(board, name, (minVoltage, maxVoltage, terminalConfiguration), channelID)
        {
            analogReader = new AnalogSingleChannelReader(MainTask.Stream);
        }

        public override void SetupChannels()
        {
            (double MinVoltage, double MaxVoltage, AITerminalConfiguration TerminalConfiguration) parameters = ((double, double, AITerminalConfiguration))ChannelParameters!;

            MainTask.AIChannels.CreateVoltageChannel(MainTask.Channels[0].ToString(), $"{MainTask.Name}_{MainTask.Channels[0].FriendlyName}", (NationalInstruments.DAQmx.AITerminalConfiguration) parameters.TerminalConfiguration, parameters.MinVoltage, parameters.MaxVoltage, AIVoltageUnits.Volts);
        }

        protected override double GetReading() => analogReader.ReadSingleSample();
    }
}


