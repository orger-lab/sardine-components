namespace Sardine.Utils.Measurements.Electric
{
    public abstract class VoltageUnit : MeasuringUnit
    {
        private static readonly UnitExponent VoltageUnitExponent = new() { Kilogram = 1, Meter = 2, Second = -3, Ampere = -1 };
        public override UnitExponent BaseUnit => VoltageUnitExponent;
    }
}
