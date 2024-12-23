namespace Sardine.Utils.Measurements
{
    public sealed class ArbitraryUnit : MeasuringUnit
    {
        public static UnitExponent ArbitraryUnitExponent { get; } = new UnitExponent();
        public override UnitExponent BaseUnit => ArbitraryUnitExponent;
        public override string Symbol => "AU";
    }
}
