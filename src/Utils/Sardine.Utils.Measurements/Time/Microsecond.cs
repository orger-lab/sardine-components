namespace Sardine.Utils.Measurements.Time
{
    public sealed class Microsecond : TimeUnit
    {
        public override double Multiplier { get; } = 1e-6;
        public override string Symbol { get; } = "us";
    }
}
