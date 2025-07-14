namespace Sardine.Devices.Hamamatsu.Camera.API
{
    public enum CaptureMode : int
    {
        Normal = 1,
        DarkCalib = 2,
        ShadingCalib = 3,
        TapGainCalib = 4,
        BackfocusCalib = 5,
    }
}