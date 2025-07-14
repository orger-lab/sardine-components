using Sardine.Utils.Measurements.Time;
using Sardine.Utils.Measurements;

namespace Sardine.Utils.Waveforms
{
    public sealed class ConstantValueWaveGenerator : WaveformGenerator, IDigitalOutputProvider
    {
        public double Level { get; set; } = 0;

        public bool[] GenerateTriggers(int patternLength, Measure<Hertz> samplingRate)
        {
            (_, int TotalLength) = GetScaleProperties(patternLength, samplingRate);
            bool[] triggers = new bool[TotalLength];
            for (int i = 0; i < TotalLength; i++)
            {
                triggers[i] = Level > 0;
            }

            return triggers;
        }

        protected override double[] GenerateWaveform(double framesPerStimCycle, int totalLength)
        {
            double[] waveform = new double[totalLength];
            for (int i = 0; i < totalLength; i++)
            {
                waveform[i] = Level;
            }

            return waveform;
        }
    }

}
