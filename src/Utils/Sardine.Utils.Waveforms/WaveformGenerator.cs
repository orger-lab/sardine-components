using Sardine.Utils.Measurements.Time;
using Sardine.Utils.Measurements;

namespace Sardine.Utils.Waveforms
{
    public abstract class WaveformGenerator : IAnalogOutputProvider
    {
        public double Frequency { get; set; } = 1;
        public double Length { get; set; } = 1;
        public FrequencyScale FrequencyScale { get; set; } = FrequencyScale.SI;

        public virtual void Reset() { }
        protected abstract double[] GenerateWaveform(double framesPerStimCycle, int totalLength);

        public double[] GenerateWaveform(int patternLength, Measure<Hertz> samplingRate)
        {
            (double framesPerStimCycle, int totalLength) = GetScaleProperties(patternLength, samplingRate);

            return GenerateWaveform(framesPerStimCycle, totalLength);
        }
        protected (double FramesPerStimCycle, int TotalLength) GetScaleProperties(int patternLength, Measure<Hertz> samplingRate)
        {
            double framesPerStimCycle = 0;
            int totalLength = 0;
            switch (FrequencyScale)
            {
                case FrequencyScale.SI:
                    framesPerStimCycle = samplingRate / Frequency;
                    totalLength = (int)(samplingRate * Length);
                    break;
                case FrequencyScale.ClockSource:
                    framesPerStimCycle = 1 / Frequency;
                    totalLength = (int)(Length);
                    break;
                case FrequencyScale.PatternLength:
                    framesPerStimCycle = patternLength / Frequency;
                    totalLength = (int)(patternLength * Length);
                    break;
            }
            if (Length == 0)
                totalLength = patternLength;

            return (framesPerStimCycle, totalLength);
        }
    }

}
