using Sardine.Utils.Waveforms;

namespace Sardine.Sequencer
{
    public interface IPatternBlock
    {
        public string BlockName { get; }
        public int NumPatterns { get; }
        public IDictionary<object, IList<IAnalogOutputProvider?>> AnalogPatterns { get; }
        public IDictionary<object, IList<IDigitalOutputProvider?>> DigitalPatterns { get; }
        public IDictionary<IVirtualSequencerOutputChannel, IList<object?>> VirtualPatterns { get; }
    }
}