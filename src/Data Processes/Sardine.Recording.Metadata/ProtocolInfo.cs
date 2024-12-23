namespace Sardine.Recording.Metadata
{
    public sealed class ProtocolInfo(string name, string path)
    {
        public string Name { get; } = name;
        public string Path { get; } = path;
    }
}
