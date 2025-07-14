using Sardine.Utils.Measurements.Time;
using Sardine.Utils.Measurements;

namespace Sardine.Utils.Waveforms
{
    public interface IAnalogOutputProvider
    {
        public double[] GenerateWaveform(int patternLength, Measure<Hertz> samplingRate);
        public void Reset();
    }

}
