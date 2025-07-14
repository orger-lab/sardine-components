using System;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    public class CameraProperty<T> : CameraProperty where T : IConvertible
    {
        private readonly DoubleToTypeConverter ToType;

        public override Type PropertyType => typeof(T);
        public T this[int index]
        {
            get => ToType(GetValueDouble(index));
            set { SetValueDouble(index, value.ToDouble(null)); }
        }
        public T Value
        {
            get => this[0];
            set => this[0] = value;
        }

        public delegate T DoubleToTypeConverter(double value);

        internal CameraProperty(DCam camera, CameraPropertyID propertyID, double? step, double? minValue, double? maxValue,
                                    double? defaultValue, string name, AttributeFlags attributes, int[] propIDVal,
                                    DoubleToTypeConverter doubleToType)
                        : base(camera, propertyID, step, minValue, maxValue, defaultValue, name, attributes, propIDVal)
        {
            ToType = doubleToType;
        }
        internal CameraProperty(DCam camera, CameraPropertyID property, DCAMPropertyAttributeStruct dataStruct,
                                         DoubleToTypeConverter doubleToType)
            : base(camera, property, dataStruct)
        {
            ToType = doubleToType;
        }

        public static explicit operator CameraProperty<int>?(CameraProperty<T>? cProp)
        {
            if (cProp == null)
                return null;

            return new CameraProperty<int>(cProp.apiReference, cProp.PropertyID, cProp.Step, cProp.MinValue, cProp.MaxValue, cProp.DefaultValue,
                                           cProp.Name, cProp.Attributes, cProp.PropertyIDValue, (x) => (int)x);
        }
        public static explicit operator CameraProperty<double>?(CameraProperty<T>? cProp)
        {
            if (cProp == null)
                return null;

            return new CameraProperty<double>(cProp.apiReference, cProp.PropertyID, cProp.Step, cProp.MinValue, cProp.MaxValue, cProp.DefaultValue,
                                           cProp.Name, cProp.Attributes, cProp.PropertyIDValue, (x) => x);
        }
        public static explicit operator CameraProperty<Mode>?(CameraProperty<T>? cProp)
        {
            if (cProp == null)
                return null;

            return new CameraProperty<Mode>(cProp.apiReference, cProp.PropertyID, cProp.Step, cProp.MinValue, cProp.MaxValue, cProp.DefaultValue,
                                           cProp.Name, cProp.Attributes, cProp.PropertyIDValue, (x) => (Mode)x);
        }
    }
}