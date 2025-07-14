namespace Sardine.Devices.Hamamatsu.Camera.API
{
    public enum TimestampProducer : int
    {
        None = 1,
        DCamModule = 2,
        KernelDriver = 3,
        CaptureDevice = 4,
        ImagingDevice = 5,
    }
}