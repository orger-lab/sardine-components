using Sardine.Utils.Measurements;

namespace Sardine.Constructs
{
    public interface ISampleSequencer<TUnit> : IRangeProvider<TUnit>, ISequencerParameterization where TUnit : MeasuringUnit, new()
    {
        public bool IsExecuting { get; }

        public event EventHandler<ExecutionStatusEventArgs>? OnExecutionStatusChanged;
        public int NumChannels { get; }
        public void Start();
        public void Stop();

        public new Measure<TUnit> MinValue { get; }
        public new Measure<TUnit> MaxValue { get; }
    }
}
