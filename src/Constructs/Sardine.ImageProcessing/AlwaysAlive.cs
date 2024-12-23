namespace Sardine.ImageProcessing
{
    public class AlwaysAlive : IAliveStatusChecker
    {
        public bool GetAliveStatus(int frameID) => true;

        public static AlwaysAlive Instance { get; } = new AlwaysAlive();
    }
}
