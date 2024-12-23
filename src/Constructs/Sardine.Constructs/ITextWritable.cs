namespace Sardine.Recording.Data.Text
{
    public interface ITextWritable
    {
        string WriteHeader();
        string WriteData();

        bool IsAlive { get; }
    }
}