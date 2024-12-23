namespace Sardine.Utils.Measurements.Electric
{
    public abstract class ChargeUnit : MeasuringUnit
    {
        private static readonly UnitExponent ChargeUnitExponent = new() { Second = 1, Ampere = 1 };
        public override UnitExponent BaseUnit => ChargeUnitExponent;
    }
}
