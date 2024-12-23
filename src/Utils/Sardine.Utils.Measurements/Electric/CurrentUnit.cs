namespace Sardine.Utils.Measurements.Electric
{
    public abstract class CurrentUnit : MeasuringUnit
    {
        private static readonly UnitExponent CurrentUnitExponent = new() { Ampere = 1 };
        public override UnitExponent BaseUnit => CurrentUnitExponent;
    }
}
