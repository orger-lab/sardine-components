namespace Sardine.Utils.Measurements.Size
{
    public abstract class DistanceUnit : MeasuringUnit
    {
        private static readonly UnitExponent DistanceUnitExponent = new() { Meter = 1 };
        public override UnitExponent BaseUnit => DistanceUnitExponent;
    }
}
