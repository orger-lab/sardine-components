using Sardine.Utils.Measurements.Time;
using Sardine.Utils.Measurements;

namespace Sardine.Utils.Waveforms
{
    public class DoubleSquareWaveGenerator : WaveformGenerator, IDigitalOutputProvider
    {
        public double DutyCycle { get; set; } = 0.5;
        public double HighLevel { get; set; } = 5;
        public double LowLevel { get; set; } = 0;
        public double Phase { get; set; } = 0;

        public double PulseDelay { get; set; } = 0.5;
        public SquareWavePolarity FirstEdge { get; set; } = SquareWavePolarity.High;

        protected override double[] GenerateWaveform(double framesPerStimCycle, int totalLength)
        {
            return GenerateBoolWaveform(framesPerStimCycle, totalLength).Select(x => x ? HighLevel : LowLevel).ToArray();
        }

        private bool[] GenerateBoolWaveform(double framesPerStimCycle, int totalLength)
        {
            bool[] waveform = new bool[totalLength];
            double framesDC = DutyCycle * framesPerStimCycle;
            for (int i = 0; i < totalLength; i++)
            {
                switch (FirstEdge)
                {
                    case SquareWavePolarity.High:
                        waveform[i] = ((i + framesPerStimCycle * Phase / (2 * Math.PI)) % framesPerStimCycle < framesDC)
                                   || ((i + framesPerStimCycle * (Phase + PulseDelay*2*Math.PI) / (2 * Math.PI)) % framesPerStimCycle < framesDC);
                        break;
                    case SquareWavePolarity.Low:
                        waveform[i] = !(((i + framesPerStimCycle * Phase / (2 * Math.PI)) % framesPerStimCycle < framesDC)
                            || ((i + framesPerStimCycle * (Phase + PulseDelay * 2 * Math.PI) / (2 * Math.PI)) % framesPerStimCycle < framesDC)) ;
                        break;
                }
            }

            return waveform;
        }

        public bool[] GenerateTriggers(int patternLength, Measure<Hertz> samplingRate)
        {
            (double FramesPerStimCycle, int TotalLength) = GetScaleProperties(patternLength, samplingRate);
            return GenerateBoolWaveform(FramesPerStimCycle, TotalLength);
        }


    }

}
