using Sardine.Utils.Measurements;

namespace Sardine.Constructs
{
    public interface ISingleAxisController
    {
        public bool IsEnabled { get; }

        public double Value { get; set; }
        public MeasuringUnit Unit { get; }

        public double GetValue();
        public bool SetValue(double value);

        public event EventHandler? OnValueChanged;
        public event EventHandler? OnEnabledStatusChanged;

        public double MinValue { get; }
        public double MaxValue { get; }
    }


    public interface ISingleAxisController<TUnit> : ISingleAxisController, IRangeProvider<TUnit> where TUnit : MeasuringUnit, new()
    {
        public new Measure<TUnit> Value
        {
            get => IsEnabled ? GetValue() : double.NaN;
            set
            {
                if (IsEnabled)
                {
                    _ = SetValue(value);
                }
            }
        }



        public new Measure<TUnit> GetValue();
        public bool SetValue(Measure<TUnit> value);

        double ISingleAxisController.Value
        {
            get => Value;
            set => Value = value;
        }
        MeasuringUnit ISingleAxisController.Unit => new TUnit();
        double ISingleAxisController.GetValue() => GetValue();
        bool ISingleAxisController.SetValue(double value) => SetValue(value);

        double ISingleAxisController.MinValue => ((IRangeProvider<TUnit>)this).MinValue;
        double ISingleAxisController.MaxValue => ((IRangeProvider<TUnit>)this).MaxValue;
    }
}
