using Sardine.Utils.Measurements;
using Sardine.Utils.Measurements.Size;

using System.Drawing;
using System.Numerics;

namespace Sardine.Optics
{
    public interface IAxialImagingDevice : IAxialIlluminator
    {
        public Telescope CollectionOptics { get; }
        public RefractiveIndex Medium { get; }
        public IAreaImagingDevice Sensor2D { get; }
        public SizeF MaximumFOV => (SizeF)((Vector2)(SizeF)Sensor2D.SensorResolution * (Vector2)Sensor2D.PixelSize) / (float)CollectionOptics.Magnification;
        public Rectangle ImagingArea { get; }

        public Measure<Meter> PositionZ { get; set; }
        double IAxialIlluminator.NA => CollectionOptics.Lens1.NA;
    }
}
