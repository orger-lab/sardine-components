namespace Sardine.Devices.Hamamatsu.Camera.API
{
    public enum CameraStatusIntensity : int
    {
        Good = 1,
        TooDark = 2,
        TooBright = 3,
        Uncare = 4,
        EMGainProtection = 5,
        InconsistentOptics = 6,
        NoData = 7,
    }
}