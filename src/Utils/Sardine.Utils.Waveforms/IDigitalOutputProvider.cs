using Sardine.Utils.Measurements.Time;
using Sardine.Utils.Measurements;

namespace Sardine.Utils.Waveforms
{
    public interface IDigitalOutputProvider
    {
        public bool[] GenerateTriggers(int patternLength, Measure<Hertz> samplingRate);
        public void Reset();
    }

}
