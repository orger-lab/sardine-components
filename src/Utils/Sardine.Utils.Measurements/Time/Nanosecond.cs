namespace Sardine.Utils.Measurements.Time
{
    public sealed class Nanosecond : TimeUnit
    {
        public override double Multiplier { get; } = 1e-9;
        public override string Symbol { get; } = "ns";
    }
}
