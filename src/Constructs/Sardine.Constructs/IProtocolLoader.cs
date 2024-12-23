namespace Sardine.Utils.Protocols
{
    public interface IProtocolLoader
    {
        public string Name { get; }
        public Dictionary<string,string> LoadProtocol(string name);
    }
}
