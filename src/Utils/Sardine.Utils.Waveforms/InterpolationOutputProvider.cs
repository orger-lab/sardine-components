using Sardine.Utils.Measurements.Time;
using Sardine.Utils.Measurements;

namespace Sardine.Utils.Waveforms
{
    public sealed class InterpolationOutputProvider(double startLevel, double endLevel) : IAnalogOutputProvider
    {
        public double StartLevel { get; } = startLevel;
        public double EndLevel { get; } = endLevel;


        public double[] GenerateWaveform(int patternLength, Measure<Hertz> samplingRate)
        {
            return MathTransforms.LinearInterpolation(StartLevel, EndLevel, patternLength).ToArray();
        }

        public void Reset() { }
    }

}
