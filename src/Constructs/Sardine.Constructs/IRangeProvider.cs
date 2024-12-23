using Sardine.Utils.Measurements;

namespace Sardine.Constructs
{
    public interface IRangeProvider<TUnit> where TUnit : MeasuringUnit, new()
    {
        public Measure<TUnit> MinValue { get; }
        public Measure<TUnit> MaxValue { get; }
    }
}
