namespace Sardine.Utils.Measurements.Time
{
    public abstract class TimeUnit : MeasuringUnit
    {
        private static readonly UnitExponent TimeUnitExponent = new() { Second = 1 };
        public override UnitExponent BaseUnit => TimeUnitExponent;
    }
}
