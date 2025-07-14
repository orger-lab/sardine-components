using System;
using System.Collections.Generic;

namespace Sardine.Devices.NI.DAQ
{
    public abstract class DAQDevice : IDisposable
    {
        protected private bool disposedValue;

        public DAQTask MainTask { get; }
        public object? ChannelParameters { get; }

        private void MainTask_OnTaskStateChanged(object? sender, NITaskStateEventArgs e)
        {
            OnTaskStateChanged?.Invoke(this, e);
        }

        public event EventHandler<NITaskStateEventArgs>? OnTaskStateChanged;

        public DAQDevice(DAQBoard board, string name, object? parameters, params DaqPhysicalChannelID[] channels) : this(board, name, parameters, (IEnumerable<DaqPhysicalChannelID>)channels) { }
        public DAQDevice(DAQBoard board, string name, object? parameters, IEnumerable < DaqPhysicalChannelID> channels)
        {
            MainTask = board.GetTask(name,channels);
            MainTask.OnTaskStateChanged += MainTask_OnTaskStateChanged;
            ChannelParameters = parameters;

            if (!MainTask.ChannelSetupComplete)
            {
                SetupChannels();
                MainTask.ChannelSetupComplete = true;
            }
        }

        public abstract void SetupChannels();

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    MainTask.Stop();
                    MainTask.Board.DeleteTask(MainTask.Name);
                    MainTask.Dispose();
                }
                disposedValue = true;
            }
        }

    }
}
