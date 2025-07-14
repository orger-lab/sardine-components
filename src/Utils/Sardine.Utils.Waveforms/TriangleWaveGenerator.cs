namespace Sardine.Utils.Waveforms
{
    public sealed class TriangleWaveGenerator : WaveformGenerator
    {
        public double HighLevel { get; set; } = 1;
        public double LowLevel { get; set; } = 0;

        protected override double[] GenerateWaveform(double framesPerStimCycle, int totalLength)
        {
            double[] waveform = new double[totalLength];

            double step = ((HighLevel - LowLevel) / (framesPerStimCycle/2 - 1));

            bool directionUp = true;
            waveform[0] = LowLevel;

            for (int i = 1; i < totalLength; i++)
            {
                if (directionUp)
                {
                    waveform[i] = waveform[i - 1] + step;
                    if ((waveform[i] >= HighLevel && HighLevel >= LowLevel) || (waveform[i] <= HighLevel && HighLevel < LowLevel))
                    {
                        waveform[i] = HighLevel;
                        directionUp = false;
                    }
                }
                else
                {

                    waveform[i] = waveform[i - 1] - step;
                    if ((waveform[i] <= LowLevel && HighLevel >= LowLevel) || (waveform[i] >= LowLevel && HighLevel < LowLevel))
                    {
                        waveform[i] = LowLevel;
                        directionUp = true;
                    }
                }
            }
            return waveform;
        }
    }
}
