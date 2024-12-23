namespace Sardine.Utils.Measurements.Time
{
    public sealed class Millisecond : TimeUnit
    {
        public override double Multiplier { get; } = 1e-3;
        public override string Symbol { get; } = "ms";
    }
}
