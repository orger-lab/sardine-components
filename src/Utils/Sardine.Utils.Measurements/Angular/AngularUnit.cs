namespace Sardine.Utils.Measurements.Angular
{
    public abstract class AngularUnit : MeasuringUnit
    {
        private static readonly UnitExponent AngularUnitExponent = new();
        public override UnitExponent BaseUnit => AngularUnitExponent;
    }
}
