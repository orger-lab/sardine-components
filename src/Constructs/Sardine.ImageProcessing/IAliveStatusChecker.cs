namespace Sardine.ImageProcessing
{
    public interface IAliveStatusChecker
    {
        public bool GetAliveStatus(int frameID);
    }
}
