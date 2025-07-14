using NationalInstruments.DAQmx;
using System;
using Sardine.Utils.Measurements;
using System.Collections.Generic;
using NationalInstruments;
using Sardine.Utils.Measurements.Time;

namespace Sardine.Devices.NI.DAQ
{
    public class DAQSingleDigitalChannelBufferedController : DAQDevice
    {
        private readonly DigitalSingleChannelWriter taskWriter;

        public DAQSingleDigitalChannelBufferedController(DAQBoard board, DaqPhysicalChannelID daqChannel, string deviceName) : base(board, deviceName, null, daqChannel)
        {
            if (daqChannel.ChannelType != PhysicalChannelTypes.DOLine)
                throw new ArgumentException(nameof(daqChannel.ChannelType));

            taskWriter = new DigitalSingleChannelWriter(MainTask.Stream);
        }

        public override void SetupChannels()
        {
            MainTask.DOChannels.CreateChannel(MainTask.Channels[0].ToString(), $"{MainTask.Name}_{MainTask.Channels[0].FriendlyName}_Out", ChannelLineGrouping.OneChannelForEachLine);
        }

        public virtual void WriteSamples(IList<bool> samples, Measure<Hertz> sampleRate, bool repeatSamples, DAQTask.DigitalEdgeStartTriggerEdge edge)
        {
            MainTask.ConfigureClock(sampleRate, samples.Count, repeatSamples: repeatSamples);
            MainTask.ConfigureStartTrigger(edge);

            if (MainTask.ExternalClock is null)
            {
                MainTask!.Triggers.StartTrigger.DelayUnits = StartTriggerDelayUnits.Seconds;
                MainTask!.Triggers.StartTrigger.Delay = DAQTask.MinimumStartTriggerDelay.ValueSI;
            }

            MainTask.Reserve();

            DigitalWaveform waveform = new(samples.Count, 1);
            for (int i = 0; i < samples.Count; i++)
            {
                waveform.Samples[i].States[0] = samples[i] ? DigitalState.ForceUp : DigitalState.ForceDown;
            }

            taskWriter.WriteWaveform(false, waveform);
        }

    }
}
