using Sardine.Sequencer;
using Sardine.Utils.Waveforms;
using System.Collections.Generic;

namespace Sardine.Devices.NI.DAQ
{
    public class DAQPatternBlock : IPatternBlock
    {
        public override string ToString() => BlockName;
        public DAQPatternBlock(string blockName, int numPatterns,
                           IDictionary<DaqPhysicalChannelID, IList<IAnalogOutputProvider?>>? analogPatterns = null,
                           IDictionary<DaqPhysicalChannelID, IList<IDigitalOutputProvider?>>? digitalPatterns = null,
                           IDictionary<IVirtualSequencerOutputChannel, IList<object?>>? virtualPatterns = null,
                           bool fillEmptyWithLast = false)
        {
            BlockName = blockName;
            NumPatterns = numPatterns;
            AnalogPatterns = (Dictionary<object, IList<IAnalogOutputProvider?>>?)analogPatterns ?? new Dictionary<object, IList<IAnalogOutputProvider?>>();
            DigitalPatterns = (Dictionary<object, IList<IDigitalOutputProvider?>>?)digitalPatterns ?? new Dictionary<object, IList<IDigitalOutputProvider?>>();
            VirtualPatterns = virtualPatterns ?? new Dictionary<IVirtualSequencerOutputChannel, IList<object?>>();
            FillEmptyWithLast = fillEmptyWithLast;
        }


        public string BlockName { get; }
        public int NumPatterns { get; }


        public IDictionary<object, IList<IAnalogOutputProvider?>> AnalogPatterns { get; }
        public IDictionary<object, IList<IDigitalOutputProvider?>> DigitalPatterns { get; }
        public IDictionary<IVirtualSequencerOutputChannel, IList<object?>> VirtualPatterns { get; }
        public bool FillEmptyWithLast { get; }
    }

}
