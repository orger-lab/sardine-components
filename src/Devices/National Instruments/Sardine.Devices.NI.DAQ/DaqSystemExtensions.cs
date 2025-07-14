using System.Collections.Generic;
using System.Linq;

using NationalInstruments.DAQmx;

namespace Sardine.Devices.NI.DAQ
{
    public static class DaqSystemExtensions
    {
        public static DaqIO GetIO(this DaqSystem daqSystem, string? deviceName=null)
        {
            deviceName ??= daqSystem.Devices[0];

            string[] _aoNames = daqSystem.GetPhysicalChannels(PhysicalChannelTypes.AO, PhysicalChannelAccess.External).Where((x) => x.Contains(deviceName)).ToArray();
            string[] _aiNames = daqSystem.GetPhysicalChannels(PhysicalChannelTypes.AI, PhysicalChannelAccess.External).Where((x) => x.Contains(deviceName)).ToArray();
            string[] _coNames = daqSystem.GetPhysicalChannels(PhysicalChannelTypes.CO, PhysicalChannelAccess.External).Where((x) => x.Contains(deviceName)).ToArray();
            string[] _ciNames = daqSystem.GetPhysicalChannels(PhysicalChannelTypes.CI, PhysicalChannelAccess.External).Where((x) => x.Contains(deviceName)).ToArray();
            string[] _dilNames = daqSystem.GetPhysicalChannels(PhysicalChannelTypes.DILine, PhysicalChannelAccess.External).Where((x) => x.Contains(deviceName)).ToArray();
            string[] _dolNames = daqSystem.GetPhysicalChannels(PhysicalChannelTypes.DOLine, PhysicalChannelAccess.External).Where((x) => x.Contains(deviceName)).ToArray();
            string[] _dipNames = daqSystem.GetPhysicalChannels(PhysicalChannelTypes.DIPort, PhysicalChannelAccess.External).Where((x) => x.Contains(deviceName)).ToArray();
            string[] _dopNames = daqSystem.GetPhysicalChannels(PhysicalChannelTypes.DOPort, PhysicalChannelAccess.External).Where((x) => x.Contains(deviceName)).ToArray();

            string[] _aoInternals = daqSystem.GetTerminals(TerminalTypes.All).Where((x) => x.Contains(deviceName)).Where((x) => x.Contains(@"ao/")).ToArray();
            string[] _aiInternals = daqSystem.GetTerminals(TerminalTypes.All).Where((x) => x.Contains(deviceName)).Where((x) => x.Contains(@"ai/")).ToArray();
            string[] _doInternals = daqSystem.GetTerminals(TerminalTypes.All).Where((x) => x.Contains(deviceName)).Where((x) => x.Contains(@"do/")).ToArray();
            string[] _diInternals = daqSystem.GetTerminals(TerminalTypes.All).Where((x) => x.Contains(deviceName)).Where((x) => x.Contains(@"di/")).ToArray();
            string[] _timebases = daqSystem.GetTerminals(TerminalTypes.All).Where((x) => x.Contains(deviceName)).Where((x) => x.Contains("Hz")).ToArray();

            DaqPhysicalChannelID[] _ao = new DaqPhysicalChannelID[_aoNames.Length];
            for (int i = 0; i < _ao.Length; i++)
            {
                _ao[i] = new DaqPhysicalChannelID(_aoNames[i], PhysicalChannelTypes.AO, PhysicalChannelAccess.External);
            }

            DaqPhysicalChannelID[] _ai = new DaqPhysicalChannelID[_aiNames.Length];
            for (int i = 0; i < _ai.Length; i++)
            {
                _ai[i] = new DaqPhysicalChannelID(_aiNames[i], PhysicalChannelTypes.AI, PhysicalChannelAccess.External);
            }

            DaqPhysicalChannelID[] _co = new DaqPhysicalChannelID[_coNames.Length];
            for (int i = 0; i < _co.Length; i++)
            {
                _co[i] = new DaqPhysicalChannelID(_coNames[i], PhysicalChannelTypes.CO, PhysicalChannelAccess.External);
            }

            DaqPhysicalChannelID[] _ci = new DaqPhysicalChannelID[_ciNames.Length];
            for (int i = 0; i < _ci.Length; i++)
            {
                _ci[i] = new DaqPhysicalChannelID(_ciNames[i], PhysicalChannelTypes.CI, PhysicalChannelAccess.External);
            }


            int caret = 0;
            DaqPhysicalChannelID[][] _di = new DaqPhysicalChannelID[_dipNames.Length][];
            Dictionary<DaqPhysicalChannelID, DaqPFI> _pfi = new();
            
            int[] _diCounters = new int[_dipNames.Length];

            for (int i = 0; i < _dilNames.Length; i++)
            {
                for (int k = 0; k < _dipNames.Length; k++)
                {
                    if (_dilNames[i].Contains(_dipNames[k]))
                        _diCounters[k]++;
                }
            }
            
            for (int i = 0; i < _diCounters.Length; i++)
            {
                _di[i] = new DaqPhysicalChannelID[_diCounters[i]];
                for (int k = 0; k < _diCounters[i]; k++)
                {
                    _di[i][k] = new DaqPhysicalChannelID(_dilNames[caret], PhysicalChannelTypes.DILine, PhysicalChannelAccess.External);
                    if (i>0)
                    {
                        _pfi.Add(_di[i][k], new DaqPFI(deviceName, _pfi.Count));
                    }
                    caret++;
                }
            }

            caret = 0;
            DaqPhysicalChannelID[][] _do = new DaqPhysicalChannelID[_dopNames.Length][];
            int[] _doCounters = new int[_dopNames.Length];

            for (int i = 0; i < _dolNames.Length; i++)
            {
                for (int k = 0; k < _dopNames.Length; k++)
                {
                    if (_dolNames[i].Contains(_dopNames[k]))
                        _doCounters[k]++;
                }
            }

            for (int i = 0; i < _doCounters.Length; i++)
            {
                _do[i] = new DaqPhysicalChannelID[_doCounters[i]];
                for (int k = 0; k < _doCounters[i]; k++)
                {
                    _do[i][k] = new DaqPhysicalChannelID(_dolNames[caret], PhysicalChannelTypes.DOLine, PhysicalChannelAccess.External);
                    caret++;
                }
            }

            DaqInternalChannelID[] _aoI = new DaqInternalChannelID[_aoInternals.Length];
            for (int i = 0; i < _aoInternals.Length; i++)
            {
                _aoI[i] = new DaqInternalChannelID(_aoInternals[i], InternalChannelAccess.AO);
            }

            DaqInternalChannelID[] _aiI = new DaqInternalChannelID[_aiInternals.Length];
            for (int i = 0; i < _aiInternals.Length; i++)
            {
                _aiI[i] = new DaqInternalChannelID(_aiInternals[i], InternalChannelAccess.AI);
            }

            DaqInternalChannelID[] _doI = new DaqInternalChannelID[_doInternals.Length];
            for (int i = 0; i < _doInternals.Length; i++)
            {
                _doI[i] = new DaqInternalChannelID(_doInternals[i], InternalChannelAccess.DO);
            }

            DaqInternalChannelID[] _diI = new DaqInternalChannelID[_diInternals.Length];
            for (int i = 0; i < _diInternals.Length; i++)
            {
                _diI[i] = new DaqInternalChannelID(_diInternals[i], InternalChannelAccess.DI);
            }

            DaqInternalChannelID[] _tb = new DaqInternalChannelID[_timebases.Length];
            for (int i = 0; i < _timebases.Length; i++)
            {
                _tb[i] = new DaqInternalChannelID(_timebases[i], InternalChannelAccess.Timebase);
            }

            return new DaqIO(_ai, _ao, _ci, _co, _di, _do, _pfi, _aoI, _aiI, _doI, _diI, _tb);
        }
    }
}
