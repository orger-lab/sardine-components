namespace Sardine.Utils.Measurements.Light
{
    public abstract class LuminosityUnit : MeasuringUnit
    {
        private static readonly UnitExponent LuminosityUnitExponent = new() { Candela = 1 };
        public override UnitExponent BaseUnit => LuminosityUnitExponent;
    }
}
