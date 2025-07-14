using NationalInstruments.DAQmx;
using Sardine.Utils.Measurements.Electric;
using Sardine.Utils.Measurements;
using System;

namespace Sardine.Devices.NI.DAQ.Controllers
{
    public class SimpleAOWriter : DAQDevice
    {
        private readonly AnalogSingleChannelWriter taskWriter;

        public int Resolution { get; }
        public Measure<Volt> MinValue { get; }
        public Measure<Volt> MaxValue { get; }


        public SimpleAOWriter(DAQBoard board, DaqPhysicalChannelID daqChannel, string deviceName, Measure<Volt>? minValue = null, Measure<Volt>? maxValue = null) : base(board, deviceName, (minValue ?? -10, maxValue ?? 10), daqChannel)
        {
            MinValue = minValue ?? -10;
            MaxValue = maxValue ?? 10;

            taskWriter = new AnalogSingleChannelWriter(MainTask.Stream);
            Resolution = (int)Math.Pow(2, MainTask.AOChannels[0].Resolution);
        }

        public override void SetupChannels()
        {
            MainTask.AOChannels.CreateVoltageChannel(MainTask.Channels[0].ToString(), $"{MainTask.Name}_{MainTask.Channels[0].FriendlyName}_Out", -10, 10, AOVoltageUnits.Volts);
            MainTask.Control(TaskAction.Verify);
        }
        
        public virtual void WriteValue(Measure<Volt> sample)
        {
            taskWriter.WriteSingleSample(true, sample);
        }

        
    }
}
