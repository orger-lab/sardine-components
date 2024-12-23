namespace Sardine.Utils.Measurements.Light
{
    public abstract class AreaIlluminationUnit : MeasuringUnit
    {
        private static readonly UnitExponent AreaIlluminationUnitExponent = new() { Candela = 1, Meter = -2 };
        public override UnitExponent BaseUnit => AreaIlluminationUnitExponent;
    }
}
