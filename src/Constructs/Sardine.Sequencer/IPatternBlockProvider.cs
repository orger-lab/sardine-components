namespace Sardine.Sequencer
{
    public interface IPatternBlockProvider
    {
        public string BlockName { get; }
        public string ShortName => BlockName.Replace(' ', '_');
        IPatternBlock GetBlock();
    }

}
