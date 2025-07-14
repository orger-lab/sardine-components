namespace Sardine.Devices.Hamamatsu.Camera.API
{
    public enum InternalTriggerHandling : int
    {
        ShorterExposureTime = 1,
        FasterFramerate = 2,
        AbandonWrongFrame = 3,
        BurstMode = 4,
        IndividualExposure = 7,
    }
}