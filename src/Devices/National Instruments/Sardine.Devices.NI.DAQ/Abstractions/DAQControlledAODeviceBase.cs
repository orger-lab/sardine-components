using NationalInstruments.DAQmx;

using Sardine.Constructs;
using System;
using Sardine.Utils.Measurements;
using Sardine.Utils.Measurements.Time;
using Sardine.Utils.Measurements.Electric;

namespace Sardine.Devices.NI.DAQ
{
    public class DAQSingleAnalogOutputController : DAQDevice
    {
        private readonly AnalogSingleChannelWriter taskWriter;


        public Measure<Nanosecond> MinimumDelay { get; } = DAQTask.MinimumStartTriggerDelay;

        public int Resolution { get; init; }
        public Measure<Hertz> MaxRate { get; }

        private bool isExecuting = false;

        public event EventHandler<ExecutionStatusEventArgs>? OnExecutionStatusChanged;

        public bool IsExecuting
        {
            get => isExecuting;
            protected internal set
            {
                if (isExecuting != value)
                {
                    isExecuting = value;
                    OnExecutionStatusChanged?.Invoke(this, new ExecutionStatusEventArgs(value));
                }
            }
        }

        public Measure<Volt> MinValue { get; }
        public Measure<Volt> MaxValue { get; }


        public DAQSingleAnalogOutputController(DAQBoard board, DaqPhysicalChannelID daqChannel, string deviceName, Measure<Volt>? minValue = null, Measure<Volt>? maxValue = null) : base(board, deviceName, (minValue ?? -10, maxValue ?? 10), daqChannel)
        {
            MinValue = minValue ?? -10;
            MaxValue = maxValue ?? 10;

            taskWriter = new AnalogSingleChannelWriter(MainTask.Stream);
            Resolution = (int)Math.Pow(2, MainTask.AOChannels[0].Resolution);
            MaxRate = new Measure<Hertz>(MainTask.Timing.SampleClockMaximumRate);
            OnTaskStateChanged += (sender, e) => { IsExecuting = MainTask.IsRunning; };
        }

        public override void SetupChannels()
        {
            MainTask.AOChannels.CreateVoltageChannel(MainTask.Channels[0].ToString(), $"{MainTask.Name}_{MainTask.Channels[0].FriendlyName}_Out", -10, 10, AOVoltageUnits.Volts);
            MainTask.Stream.WriteRegenerationMode = WriteRegenerationMode.AllowRegeneration;
            MainTask.Control(TaskAction.Verify);
            MainTask!.Triggers.StartTrigger.DelayUnits = StartTriggerDelayUnits.Seconds;
        }


        public virtual void WriteSamples(SampleSequence<Volt> samples, Measure<Hertz> sampleRate, bool repeatSamples)
        {
            if (IsExecuting == true)
                throw new Exception();

            MainTask.ConfigureClock(sampleRate, samples.Count, repeatSamples: repeatSamples);
            MainTask.ConfigureStartTrigger();

            if (MainTask.ExternalClock is null)
            {
                MainTask!.Triggers.StartTrigger.DelayUnits = StartTriggerDelayUnits.Seconds;
                MainTask!.Triggers.StartTrigger.Delay = DAQTask.MinimumStartTriggerDelay.ValueSI;
            }

            taskWriter.WriteMultiSample(false, samples.GetValues());
        }
    }
}
