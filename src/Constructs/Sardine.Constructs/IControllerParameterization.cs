using Sardine.Utils.Measurements;
using Sardine.Utils.Measurements.Time;

namespace Sardine.Constructs
{
    public interface ISequencerParameterization
    {
        public Measure<Hertz> MaxRate { get; }
        public Measure<Nanosecond> MinimumDelay { get; }
        public int Resolution { get; }
    }
}
