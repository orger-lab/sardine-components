namespace Sardine.Utils.Measurements.Angular
{
    public sealed class Degree : AngularUnit
    {
        public override double Multiplier { get; } = Math.PI / 180;
        public override string Symbol { get; } = "deg";
    }
}
