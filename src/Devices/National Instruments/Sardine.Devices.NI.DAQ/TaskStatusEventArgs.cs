using System;

namespace Sardine.Devices.NI.DAQ
{
    public sealed class NITaskStateEventArgs : EventArgs
    {
        public TaskState NewState { get; }
        public TaskState OldState { get; }

        public bool ChannelsAvailable { get; }
        public NITaskStateEventArgs(TaskState newState, TaskState oldState, bool channelsAvailable)
        {
            NewState = newState;
            OldState = oldState;
            ChannelsAvailable = channelsAvailable;
        }
    }
}
