namespace Sardine.Utils.Measurements.Size
{
    public sealed class Meter : DistanceUnit
    { }

    public sealed class Micrometer : DistanceUnit
    {
        public override double Multiplier { get; } = 1e-6;
        public override string Symbol { get; } = "um";
    }

    public sealed class Dioptry : MeasuringUnit
    {
        public override UnitExponent BaseUnit { get; } = new() { Meter = -1 };
        public override string Symbol { get; } = "dpt";
    }

    public abstract class Pixel<T> : DistanceUnit where T : DistanceUnit, new()
    {
        public override double Multiplier { get; }
        public override string Symbol { get; } = "px";

        protected Pixel(Measure<T> pixelSize)
        {
            Multiplier = pixelSize.ValueSI;
        }
    }
}
