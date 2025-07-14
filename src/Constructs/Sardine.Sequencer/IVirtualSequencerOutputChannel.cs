namespace Sardine.Sequencer
{
    public interface IVirtualSequencerOutputChannel
    {
        public string Name { get; }
        public void Actuate(object data);
    }
}