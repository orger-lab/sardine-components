using Sardine.Utils.Measurements.Time;
using Sardine.Utils.Measurements;

namespace Sardine.Sequencer
{
    public interface ISequencer
    {
        public void Start(int stopAfterNPatterns = 0);
        public void Stop(bool forced = true);
        public void Arm(Measure<Hertz> sampleRate, int patternLength, IList<(int, IPatternBlock)> blockSequence);
        public void Disarm();

        public bool IsArmed { get; }
        public bool IsRunning { get; } 
        public int CurrentPattern { get; }
        public int TotalSequenceLength { get; }
        public bool CollectReadouts { get; set; }
        public Measure<Hertz> ExecutionRate { get; }

    }
}
