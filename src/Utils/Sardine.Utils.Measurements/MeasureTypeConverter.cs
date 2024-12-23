using System.ComponentModel;
using System.Globalization;


namespace Sardine.Utils.Measurements
{
    public class MeasureTypeConverter(Type type) : TypeConverter()
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
}
