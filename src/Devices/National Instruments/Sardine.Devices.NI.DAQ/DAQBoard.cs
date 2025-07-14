using NationalInstruments.DAQmx;

using System;
using System.Collections.Generic;
using System.Linq;


namespace Sardine.Devices.NI.DAQ
{
    public sealed class DAQBoard : IDisposable
    {
        private bool disposedValue;

        Device DeviceHandle { get; }
        Dictionary<string, DAQTask> Tasks { get; } = new Dictionary<string, DAQTask>();
        public DaqIO IO { get; }
        public string DriverVersion { get;  }
        public string DeviceName { get;  }
        public string SerialNumber { get;  }
        public string ProductType { get;  }
        public DaqPhysicalChannelCollection ActiveChannels { get; private set; }

        public event EventHandler? OnTaskCollectionChanged;

        private DAQBoard(DaqIO io, string driverVersion, string deviceName,string sn, string produtType, Device deviceHandle)
        {
            IO = io;
            DriverVersion = driverVersion;
            DeviceName = deviceName;
            SerialNumber = sn;
            ProductType = produtType;
            DeviceHandle = deviceHandle;
            ActiveChannels = DaqPhysicalChannelCollection.GenerateChannelCollection(null);
        }


        public DaqPhysicalChannelID GetChannel(string name)
        {
            return ActiveChannels.Where(x => x.FriendlyName == name).First();
        }

        public DAQTask GetTask(string name, DaqPhysicalChannelID channel)
        {
            return GetTask(name, new DaqPhysicalChannelID[1] { channel });
        }

        public DAQTask GetTask(string name, IEnumerable<DaqPhysicalChannelID>? channels = null)
        {
            if (!Tasks.ContainsKey(name))
            {
                if (channels is null)
                    throw new ArgumentException("Nonexistent Task - channels not supplied", nameof(channels));
                Tasks.Add(name, new DAQTask(this, channels, name));
            }
                
            OnTaskCollectionChanged?.Invoke(this, EventArgs.Empty);


            DAQTask taskToReturn = Tasks[name];

            if (channels is not null && taskToReturn.Channels.Count() > 0)
                if (channels.Except(taskToReturn.Channels.Channels).Any() || taskToReturn.Channels.Channels.Except(channels).Any())
                {
                    if (taskToReturn.State != TaskState.Running)
                    {
                        DeleteTask(name);
                        Tasks.Add(name, new DAQTask(this, channels, name));
                    }
                    else
                    {
                        throw new Exception("Channels are weird between two task requests."); 
                    }
                }

            return Tasks[name];
        }

        public static DAQBoard GetDAQInfo(int id = 0)
        {
            string deviceName = DaqSystem.Local.Devices[id];
            string driverVersion = $"{DaqSystem.Local.DriverMajorVersion}.{DaqSystem.Local.DriverMajorVersion}.{DaqSystem.Local.DriverUpdateVersion}";

            DaqIO io = DaqSystem.Local.GetIO(deviceName);
            Device device = DaqSystem.Local.LoadDevice(deviceName);

            device.SelfTest();

            string sn = device.SerialNumber.ToString();
            string productType = device.ProductType;

            DAQBoard board = new(io, driverVersion, deviceName, sn, productType, device);

            //Current = board;

            return board;
        }

        public void SetActiveChannels(object caller)
        {
            ActiveChannels = DaqPhysicalChannelCollection.GenerateChannelCollection(caller);

            foreach(var channel in ActiveChannels)
                channel.OnReservedStatusChanged += (_, _) => OnTaskCollectionChanged?.Invoke(this, EventArgs.Empty);
        }
        public static int GetDAQCount()
        {
            return DaqSystem.Local.Devices.Length;
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DeviceHandle.Reset();
                    DeviceHandle.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        internal void DeleteTask(string name)
        {
            Tasks.Remove(name);
            OnTaskCollectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
