using Sardine.Utils.Measurements;

namespace Sardine.Constructs
{
    public interface ICalibration
    {
        public Measure<T> Calibrate<T>(Measure<T> value) where T : MeasuringUnit, new();
        public double Calibrate(double value);
        public event EventHandler? OnCalibrationParameterChanged;
    }
}
