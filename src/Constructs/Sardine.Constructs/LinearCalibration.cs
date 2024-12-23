using Sardine.Utils.Measurements;

namespace Sardine.Constructs
{
    public class LinearCalibration : ICalibration
    {
        private double sensitivity;
        private double baseline;

        public event EventHandler? OnCalibrationParameterChanged;

        public double Sensitivity
        {
            get => sensitivity;
            set
            {
                if (sensitivity != value)
                {
                    sensitivity = value;
                    OnCalibrationParameterChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        public double Baseline
        {
            get => baseline;
            set
            {
                if (baseline != value)
                {
                    baseline = value;
                    OnCalibrationParameterChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        public static LinearCalibration None => new(0, 1);
        public LinearCalibration(double baseline = 0, double sensitivity = 1)
        {
            Baseline = baseline;
            Sensitivity = sensitivity;
        }

        public double Calibrate(double value) => (value * Sensitivity) + Baseline;
        public Measure<T> Calibrate<T>(Measure<T> value) where T : MeasuringUnit, new() => (value * Sensitivity) + Baseline;

        public LinearCalibration InverseCalibration => new(-Baseline / Sensitivity, 1 / Sensitivity);
    }
}
