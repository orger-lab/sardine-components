namespace Sardine.Utils.Measurements
{
    public abstract class MeasuringUnit
    {
        public static MeasuringUnit DefaultUnit { get; } = new ArbitraryUnit();

        public abstract UnitExponent BaseUnit { get; }
        public virtual double Multiplier => 1;
        public virtual double Adder => 0;
        public virtual string Symbol => BaseUnit.ToString();

        public override string ToString() => Symbol;

        public double ConvertValueToSI(double value) => (value * Multiplier) + Adder;

        public double ConvertValueFromSI(double value) => (value - Adder) / Multiplier;

        public static Measure<T> GetMeasure<T>(double value) where T : MeasuringUnit, new() => new(value);

        public IMeasure GetMeasure(double value) => (IMeasure)Activator.CreateInstance(typeof(Measure<>).MakeGenericType(GetType()), value)!;
    }
}
