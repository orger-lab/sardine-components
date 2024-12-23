namespace Sardine.Constructs
{
    public interface ICalibrationProvider<TCalibration> where TCalibration : ICalibration
    {
        public TCalibration Calibration { get; }
    }
}
