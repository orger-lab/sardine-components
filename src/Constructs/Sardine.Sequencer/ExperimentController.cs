using Sardine.Utils.Measurements;
using Sardine.Utils.Measurements.Time;

namespace Sardine.Sequencer
{
    public class ExperimentController(ISequencer sequencer, IList<IPatternBlockProvider>? providers = null)
    {
        public double FrameRate
        {
            get; set;
        } = 1;

        public Measure<Hertz> BaseSampleRate => 10000;

        public IReadOnlyList<IPatternBlockProvider> BlockProviderList { get; } = (IReadOnlyList<IPatternBlockProvider>)(providers ?? []);
        public ISequencer Sequencer { get; } = sequencer;

        public List<(int, IPatternBlockProvider)> BlockSequence { get; set; } = [];

        public void ArmSequencer()
        {
            int pointsPerFrame = (int)Math.Floor(BaseSampleRate / FrameRate);
            double samplingRateEffective = pointsPerFrame * FrameRate;
            Sequencer.Arm(samplingRateEffective, pointsPerFrame, BlockSequence.Select((x) => (x.Item1,x.Item2.GetBlock())).ToList());
        }
    }
}
