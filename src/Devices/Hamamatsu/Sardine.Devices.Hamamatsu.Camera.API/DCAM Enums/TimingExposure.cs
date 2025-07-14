namespace Sardine.Devices.Hamamatsu.Camera.API
{
    public enum TimingExposure : int
    {
        AfterReadout = 1,
        OverlapReadout = 2,
        Rolling = 3,
        Always = 4,
        TDI = 5,
    }
}