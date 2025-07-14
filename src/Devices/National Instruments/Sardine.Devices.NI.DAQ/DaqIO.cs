using System.Collections.Generic;

namespace Sardine.Devices.NI.DAQ
{
    public sealed class DaqIO
    {
        public DaqPhysicalChannelID[] AnalogIn { get; }
        public DaqPhysicalChannelID[] AnalogOut { get; }
        public DaqPhysicalChannelID[] CounterIn { get; }
        public DaqPhysicalChannelID[] CounterOut { get; }
        public DaqPhysicalChannelID[][] DigitalIn { get; }
        public DaqPhysicalChannelID[][] DigitalOut { get; }
        public Dictionary<DaqPhysicalChannelID, DaqPFI> PFI { get; }
        public Dictionary<InternalChannelAccess, Dictionary<string, DaqInternalChannelID>> InternalLines { get; }
        
        internal DaqIO(DaqPhysicalChannelID[] _ai, DaqPhysicalChannelID[] _ao,
                       DaqPhysicalChannelID[] _ci, DaqPhysicalChannelID[] _co,
                       DaqPhysicalChannelID[][] _di, DaqPhysicalChannelID[][] _do,
                       Dictionary<DaqPhysicalChannelID, DaqPFI> _pfi,
                       DaqInternalChannelID[] _aoI,
                       DaqInternalChannelID[] _aiI,
                       DaqInternalChannelID[] _doI,
                       DaqInternalChannelID[] _diI,
                       DaqInternalChannelID[] _tb)
        {
            AnalogIn = _ai;
            AnalogOut = _ao;
            CounterIn = _ci;
            CounterOut = _co;
            DigitalIn = _di;
            DigitalOut = _do;
            PFI = _pfi;

            InternalLines = new Dictionary<InternalChannelAccess, Dictionary<string, DaqInternalChannelID>>
            {
                {InternalChannelAccess.AO, new Dictionary<string, DaqInternalChannelID>() },
                {InternalChannelAccess.AI, new Dictionary<string, DaqInternalChannelID>() },
                {InternalChannelAccess.DO, new Dictionary<string, DaqInternalChannelID>() },
                {InternalChannelAccess.DI, new Dictionary<string, DaqInternalChannelID>() },
                {InternalChannelAccess.Timebase, new Dictionary<string, DaqInternalChannelID>() },
            };

            foreach(DaqInternalChannelID channel in _aoI)
            {
                InternalLines[InternalChannelAccess.AO].Add(channel.ChannelName.Split('/')[^1], channel);
            }
            foreach (DaqInternalChannelID channel in _aiI)
            {
                InternalLines[InternalChannelAccess.AI].Add(channel.ChannelName.Split('/')[^1], channel);
            }
            foreach (DaqInternalChannelID channel in _doI)
            {
                InternalLines[InternalChannelAccess.DO].Add(channel.ChannelName.Split('/')[^1], channel);
            }
            foreach (DaqInternalChannelID channel in _diI)
            {
                InternalLines[InternalChannelAccess.DI].Add(channel.ChannelName.Split('/')[^1], channel);
            }
            foreach (DaqInternalChannelID channel in _tb)
            {
                InternalLines[InternalChannelAccess.Timebase].Add(channel.ChannelName.Split('/')[^1], channel);
            }
        }
    }
}
