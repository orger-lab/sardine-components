namespace Sardine.Sequencer
{
    public class OnCaptureFinishedEventArgs(bool forcedFinish)
    {
        public bool ForcedFinish { get; } = forcedFinish;
    }
}
