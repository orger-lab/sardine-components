using System.ComponentModel;
using System.Globalization;

namespace Sardine.Utils.Measurements
{
    public class MeasureConverter(Type type) : TypeConverter()
    {
        private readonly Type typeStore = type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(Measure<>)
                && type.GetGenericArguments().Length == 1
                ? type
                : throw new ArgumentException("Incompatible type", nameof(type));

        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(double)
|| sourceType == typeof(int) || sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) => destinationType == typeof(string) || destinationType == typeof(double) || base.CanConvertTo(context, destinationType);


        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string s)
            {
                double dValue = double.Parse(s, CultureInfo.InvariantCulture);
                return Activator.CreateInstance(typeStore, dValue);
            }

            return value is double d ? Activator.CreateInstance(typeStore, d) : base.ConvertFrom(context, culture, value);
        }

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (value is IMeasure m)
            {
                if (destinationType == typeof(double))
                {
                    return m.Value;
                }
                if (destinationType == typeof(string))
                {
                    return $"{m.Value:0.#####}";
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }


    public interface IMeasure : ICloneable
    {
        public double ValueSI
        {
            get;
            set;
        }

        public double Value { get; set; }
        public MeasuringUnit Unit { get; }

        public event EventHandler? OnValueChanged;
    }

    public static partial class MeasureMath
    {
        public static Measure<T> Max<T>(Measure<T> m1, Measure<T> m2) where T : MeasuringUnit, new() => new(Math.Max(m1.Value, m2.Value));

        public static Measure<T> Min<T>(Measure<T> m1, Measure<T> m2) where T : MeasuringUnit, new() => new(Math.Min(m1.Value, m2.Value));
    }

    [TypeConverter(typeof(MeasureConverter))]
    public sealed class Measure<TUnit> : IMeasure, IComparable<Measure<TUnit>>, IEquatable<Measure<TUnit>> where TUnit : MeasuringUnit, new()
    {
        private double valueContainer;

        public double ValueSI
        {
            get => UnitInstance.ConvertValueToSI(Value);
            set => Value = UnitInstance.ConvertValueFromSI(value);
        }

        public double Value
        {
            get => valueContainer;
            set
            {
                if (valueContainer != value)
                {
                    valueContainer = value;
                    OnValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler? OnValueChanged;

        private static TUnit UnitInstance { get; } = new();

        public MeasuringUnit Unit { get; } = UnitInstance;

        public override string ToString() => $"{Value} {Unit}";

        public static Measure<TUnit> FromSI(double value) => new(new TUnit().ConvertValueFromSI(value));

        public static Measure<TUnit>? TryConvertValue(IMeasure measureFrom)
        {
            return !measureFrom.Unit.BaseUnit.CanConvertTo(UnitInstance.BaseUnit, out int signal)
                ? null
                : (Measure<TUnit>)UnitInstance.ConvertValueFromSI(signal == 1 ? measureFrom.Unit.ConvertValueToSI(measureFrom.Value) : 1 / measureFrom.Unit.ConvertValueToSI(measureFrom.Value));
        }

        public static Measure<TUnit>? TryConvertValue<TUnitIn>(double measureFrom) where TUnitIn : MeasuringUnit, new()
        {
            TUnitIn otherUnit = new();
            return !otherUnit.BaseUnit.CanConvertTo(UnitInstance.BaseUnit, out int signal)
                ? null
                : (Measure<TUnit>)UnitInstance.ConvertValueFromSI(signal == 1 ? otherUnit.ConvertValueToSI(measureFrom) : 1 / otherUnit.ConvertValueToSI(measureFrom));
        }

        public Measure<TUnitOut>? TryConversionTo<TUnitOut>() where TUnitOut : MeasuringUnit, new()
        {
            return !Unit.BaseUnit.CanConvertTo(UnitInstance.BaseUnit, out int signal)
                ? null
                : (Measure<TUnitOut>)UnitInstance.ConvertValueFromSI(signal == 1 ? Unit.ConvertValueToSI(Value) : 1 / Unit.ConvertValueToSI(Value));
        }

        public Measure<TUnitOut> ConvertTo<TUnitOut>() where TUnitOut : MeasuringUnit, new() => TryConversionTo<TUnitOut>() ?? throw new Exception();

        public Measure()
        {
            _ = new Measure<TUnit>(double.NaN);
        }

        public bool Equals(Measure<TUnit>? other) => Equals((IMeasure?)other);

        public bool Equals(IMeasure? other) => other is not null && GetHashCode() == other.GetHashCode();



        public static implicit operator double(Measure<TUnit> measure)
        {
            return measure.Value;
        }

        public static implicit operator Measure<TUnit>(double value)
        {
            return new Measure<TUnit>(value);
        }

        public static Measure<TUnit> operator +(Measure<TUnit> val1, Measure<TUnit> val2)
        {
            return new Measure<TUnit>(val1.Value + val2.Value);
        }

        public static Measure<TUnit> operator -(Measure<TUnit> val1, Measure<TUnit> val2)
        {
            return new Measure<TUnit>(val1.Value - val2.Value);
        }

        public static bool operator <(Measure<TUnit> measure1, Measure<TUnit> measure2)
        {
            return measure1.Value < measure2.Value;
        }

        public static bool operator >(Measure<TUnit> measure1, Measure<TUnit> measure2)
        {
            return measure1.Value > measure2.Value;
        }

        public static bool operator <=(Measure<TUnit> measure1, Measure<TUnit> measure2)
        {
            return measure1.Value <= measure2.Value;
        }

        public static bool operator >=(Measure<TUnit> measure1, Measure<TUnit> measure2)
        {
            return measure1.Value >= measure2.Value;
        }

        public static bool operator ==(Measure<TUnit> measure1, IMeasure measure2)
        {
            return measure1.Equals(measure2);
        }

        public static bool operator !=(Measure<TUnit> measure1, IMeasure measure2)
        {
            return !measure1.Equals(measure2);
        }

        public static bool operator ==(Measure<TUnit> measure1, Measure<TUnit> measure2)
        {
            return measure1.Equals(measure2);
        }

        public static bool operator !=(Measure<TUnit> measure1, Measure<TUnit> measure2)
        {
            return !measure1.Equals(measure2);
        }

        public Measure(double value)
        {
            Value = value;
        }

        public override bool Equals(object? obj) => Equals(obj as Measure<TUnit>);

        public override int GetHashCode() => HashCode.Combine(Unit.BaseUnit, ValueSI);

        public object Clone() => new Measure<TUnit>(Value);

        public int CompareTo(Measure<TUnit>? other) => other is null ? throw new ArgumentNullException(nameof(other)) : Value.CompareTo(other.Value);
    }
}