namespace Sardine.Utils.Measurements.Size
{
    public abstract class AreaUnit : MeasuringUnit
    {
        private static readonly UnitExponent AreaUnitExponent = new() { Meter = 2 };
        public override UnitExponent BaseUnit => AreaUnitExponent;
    }
}
