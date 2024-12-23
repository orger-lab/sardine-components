namespace Sardine.Utils.Measurements
{
    public interface IMeasureProvider<T> where T : MeasuringUnit, new()
    {
        public Measure<T> Value { get; }
    }
}
