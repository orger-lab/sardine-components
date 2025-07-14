namespace Sardine.Utils.Waveforms
{
    public sealed class SawtoothWaveformGenerator : WaveformGenerator
    {
        public double StartLevel { get; set; } = 1;
        public double EndLevel { get; set; } = 0;
        public double Phase { get; set; } = 0;

        protected override double[] GenerateWaveform(double framesPerStimCycle, int totalLength)
        {
            double[] waveform = new double[totalLength];

            double step = ((EndLevel - StartLevel) / (framesPerStimCycle - 1));
            for (int i = 0; i < totalLength; i++)
            {
                waveform[i] = StartLevel +  step * ((i + framesPerStimCycle * Phase / (2 * Math.PI)) % framesPerStimCycle);
            }

            return waveform;
        }
    }
}
