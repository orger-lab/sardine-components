namespace Sardine.Devices.Hamamatsu.Camera.API
{
    public enum SensorCoolerStatus : int
    {
        Error4 = -4,
        Error3 = -3,
        Error2 = -2,
        Error1 = -1,
        None = 0,
        Off = 1,
        Ready = 2,
        Busy = 3,
        Always = 4,
        Warning = 5,
    }
}