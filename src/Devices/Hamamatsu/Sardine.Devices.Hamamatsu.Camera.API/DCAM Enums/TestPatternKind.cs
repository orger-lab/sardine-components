namespace Sardine.Devices.Hamamatsu.Camera.API
{
    public enum TestPatternKind : int
    {
        Flat = 2,
        HorizontalGradation = 4,
        IHorizontalGradation = 5,
        VerticalGradation = 6,
        IVerticalGradation = 7,
        Line = 8,
        Diagonal = 10,
        IDiagonal = 11,
        Framecount = 12,
    }
}