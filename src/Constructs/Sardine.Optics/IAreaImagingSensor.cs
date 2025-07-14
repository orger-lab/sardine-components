using System.Drawing;

namespace Sardine.Optics
{
    public interface IAreaImagingDevice
    {
        public Size SensorResolution { get; }
        public SizeF PixelSize { get; }
        public double Magnification { get; }
    }
}