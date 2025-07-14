namespace Sardine.Devices.Hamamatsu.Camera.API
{
    public enum CameraStatusCalibration : int
    {
        Done = 1,
        NotYet = 2,
        NoTrigger = 3,
        TooFrequentTrigger = 4,
        OutOfAdjustableRange = 5,
        UnsuitableTable = 6,
        TooDark = 7,
        TooBright = 8,
        ObjectNotDetected = 9,
    }
}