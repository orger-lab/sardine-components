namespace Sardine.Devices.Hamamatsu.Camera.API
{
    public enum CaptureStatus : int
    {
        Error = 0x0000,
        Busy = 0x0001,
        Ready = 0x0002,
        Stable = 0x0003,
        Unstable = 0x0004,
        Offline = -1,
    }
}