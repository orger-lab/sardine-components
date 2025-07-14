using NationalInstruments.DAQmx;

using Sardine.Utils.Measurements;
using Sardine.Utils.Measurements.Time;

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Sardine.Devices.NI.DAQ
{
    public class DAQTask : Task
    {
        private TaskState state = TaskState.Unverified;
        private bool channelsAvailable;

        public event EventHandler<NITaskStateEventArgs>? OnTaskStateChanged;

        public string Name { get; }
        public DAQBoard Board { get; }

        public bool ChannelSetupComplete { get; set; }

        public DaqPFI? StartTrigger { get; set; }
        public DaqPFI? ExternalClock { get; set; }


        public static Measure<Nanosecond> MinimumStartTriggerDelay { get; } = new Measure<Nanosecond>(20);

        public bool IsRunning { get; private set; }

        public bool ChannelsAvailable
        {
            get => channelsAvailable;
            private set
            {
                channelsAvailable = value;
                OnTaskStateChanged?.Invoke(this, new NITaskStateEventArgs(State, State, value));
                Debug.WriteLine($"{Name} channel availability : {(value ? "" : "not")} available");
            }
        }

        public TaskState State
        {
            get => state;
            private set
            {
                if (state != value)
                {

                    if (value == TaskState.Running)
                        IsRunning = true;
                    else
                        IsRunning = false;

                    OnTaskStateChanged?.Invoke(this, new NITaskStateEventArgs(value, state, ChannelsAvailable));
                    Debug.WriteLine($"{Name} DAQ state changed: {state} => {value}");
                    state = value;
                }
            }
        }

        TaskState OldTaskState { get; set; } = TaskState.Unverified;


        private void RevertState()
        {
            State = OldTaskState;
            if (State == TaskState.Verified)
            {
                foreach (DaqPhysicalChannelID channel in Channels)
                {
                    channel.ReservedBy = null;
                }
            }
        }

        public new void Control(TaskAction action)
        {
            switch (action)
            {
                case TaskAction.Start:
                    if (!ChannelsAvailable)
                        return;

                    if (State == TaskState.Unverified)
                        Verify();

                    OldTaskState = State;
                    State = TaskState.Running;
                    foreach (DaqPhysicalChannelID channel in Channels)
                    {
                        channel.ReservedBy = this;
                    }
                    break;
                case TaskAction.Stop:
                    RevertState();
                    break;
                case TaskAction.Commit:
                    State = TaskState.Commited;
                    break;
                case TaskAction.Reserve:
                    if (!ChannelsAvailable)
                        return;
                    State = TaskState.Reserved;
                    foreach (DaqPhysicalChannelID channel in Channels)
                    {
                        channel.ReservedBy = this;
                    }
                    break;
                case TaskAction.Verify:
                case TaskAction.Unreserve:
                case TaskAction.Abort:
                    State = TaskState.Verified;
                    foreach (DaqPhysicalChannelID channel in Channels)
                    {
                        channel.ReservedBy = null;
                    }
                    break;
            }
            try
            {
                base.Control(action);
            }
            catch (Exception ex)
            {

            }
        }
        public new void Start()
        {
            if (!ChannelsAvailable)
                return;

            Done += DAQTask_Done;
            Control(TaskAction.Start);
        }

        public new void Stop()
        {
            Control(TaskAction.Stop);
            Control(TaskAction.Unreserve);
            Done -= DAQTask_Done;
        }

        public void Abort()
        {
            Control(TaskAction.Abort);
            Done -= DAQTask_Done;

        }

        public void Verify()
        {
            if (!ChannelsAvailable)
                return;

            Control(TaskAction.Verify);
        }

        public void Reserve()
        {
            if (!ChannelsAvailable)
                return;

            Control(TaskAction.Reserve);
        }

        public void Unreserve()
        {
            Control(TaskAction.Unreserve);
        }

        public void Commit()
        {
            Control(TaskAction.Commit);
        }


        public DaqPhysicalChannelCollection Channels { get; }

        public DAQTask(DAQBoard board, IEnumerable<DaqPhysicalChannelID> channels, string name) : base(name)
        {
            Name = name;
            Board = board;
            Channels = new DaqPhysicalChannelCollection(channels);
            Channels.DaqPhysicalCollectionReservedStatusChanged += Channels_DaqPhysicalCollectionReservedStatusChanged;
            ChannelsAvailable = Channels.AreChannelsAvailable(this);
        }

        private void Channels_DaqPhysicalCollectionReservedStatusChanged(object? sender, EventArgs e)
        {
            ChannelsAvailable = Channels.AreChannelsAvailable(this);
        }

        private void DAQTask_Done(object sender, TaskDoneEventArgs e)
        {
            Console.WriteLine($"Task {Name} is done.");
            Stop();
        }

        public void ConfigureStartTrigger(DigitalEdgeStartTriggerEdge edge = DigitalEdgeStartTriggerEdge.Rising)
        {
            if (StartTrigger is null)
                Triggers.StartTrigger.ConfigureNone();
            else
                Triggers.StartTrigger.ConfigureDigitalEdgeTrigger(StartTrigger.ToString(), (NationalInstruments.DAQmx.DigitalEdgeStartTriggerEdge)edge);

            Verify();
        }
        public void ConfigureClock(double rate, int samplesPerChannel, SampleClockActiveEdge edge = SampleClockActiveEdge.Rising, bool repeatSamples = false)
        {
            Timing.ConfigureSampleClock(ExternalClock == null ? string.Empty : ExternalClock.ToString(),
                                                 rate,
                                                 (NationalInstruments.DAQmx.SampleClockActiveEdge)edge,
                                                 repeatSamples ? SampleQuantityMode.ContinuousSamples : SampleQuantityMode.FiniteSamples, samplesPerChannel);
            Verify();
        }

        public enum SampleClockActiveEdge
        {
            Rising = 10280,
            Falling = 10171
        }

        public enum DigitalEdgeStartTriggerEdge
        {
            Rising = 10280,
            Falling = 10171
        }



        public void ConfigureClock(TimeSpan period, int samplesPerChannel, SampleClockActiveEdge edge = SampleClockActiveEdge.Rising, bool pulseMode = true)
        {
            ConfigureClock(1000 / period.TotalMilliseconds, samplesPerChannel, edge, pulseMode);
        }

        

        protected override void Dispose(bool disposed = true)
        {
            base.Dispose(disposed);
            try
            {
                Board.DeleteTask(Name);
            }
            catch { }
        }

        ~DAQTask()
        {
            Dispose(disposed: false);
        }
    }
}
