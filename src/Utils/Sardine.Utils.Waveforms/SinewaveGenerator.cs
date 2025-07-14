namespace Sardine.Utils.Waveforms
{

    public sealed class SinewaveGenerator : WaveformGenerator
    {
        public double HighLevel { get; set; } = 1;
        public double LowLevel { get; set; } = 0;
        public double Phase { get; set; } = 0;

        protected override double[] GenerateWaveform(double framesPerStimCycle, int totalLength)
        {
            double[] waveform = new double[totalLength];

            double amplitude = (HighLevel - LowLevel);
            double center = HighLevel - amplitude / 2;

            for (int i = 0; i < totalLength; i++)
            {
                waveform[i] = center+amplitude*0.5*Math.Sin(i*(Math.PI*2)/ framesPerStimCycle + Phase);
            }

            return waveform;
        }
    }
}
