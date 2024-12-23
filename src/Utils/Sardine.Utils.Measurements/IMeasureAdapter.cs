namespace Sardine.Utils.Measurements
{
    public interface IMeasureAdapter<T> where T : MeasuringUnit, new()
    {
        public Measure<T> Value { set; }
    }
}
