namespace Sardine.Utils.Measurements.Time
{
    public abstract class RateUnit : MeasuringUnit
    {
        private static readonly UnitExponent RateUnitExponent = new() { Second = -1 };
        public override UnitExponent BaseUnit => RateUnitExponent;
    }
}
