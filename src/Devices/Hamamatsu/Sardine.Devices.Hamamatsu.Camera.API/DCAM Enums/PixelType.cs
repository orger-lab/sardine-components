namespace Sardine.Devices.Hamamatsu.Camera.API
{
    public enum PixelType : int
    {
        Mono8 = 0x00000001,
        Mono16 = 0x00000002,
        Mono12 = 0x00000003,
        Mono12P = 0x00000005,
        RGB24 = 0x00000021,
        RGB48 = 0x00000022,
        BGR24 = 0x00000029,
        BGR48 = 0x0000002a,
        None = 0x00000000,
    }
}