using System;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    public abstract partial class CameraProperty
    {
        internal readonly DCam apiReference;
        internal readonly int[] PropertyIDValue;

        public abstract Type PropertyType { get; }
        public CameraPropertyID PropertyID { get; }
        public string Name { get; }
        public double? MinValue { get; }
        public double? MaxValue { get; }
        public double? DefaultValue { get; }
        public double? Step { get; }
        public PropertyUnit MeasurementUnit { get; }
        public AttributeFlags Attributes { get; }

        internal CameraProperty(DCam camera, CameraPropertyID propertyID, double? step, double? minValue, double? maxValue,
                                    double? defaultValue, string name, AttributeFlags attributes, int[] propIDVal)
        {
            apiReference = camera;
            PropertyID = propertyID;
            Step = step;
            MinValue = minValue;
            MaxValue = maxValue;
            DefaultValue = defaultValue;
            Name = name;
            Attributes = attributes;
            PropertyIDValue = propIDVal;
        }
        internal CameraProperty(DCam camera, CameraPropertyID property, DCAMPropertyAttributeStruct dataStruct)
        {
            CameraPropertyAttribute cameraPropAttribute = new((uint)dataStruct.Attribute);
            apiReference = camera;
            PropertyID = property;
            Step = cameraPropAttribute.HasAttribute(CameraPropertyAttribute.HASSTEP) ? dataStruct.ValueStep : null;
            MinValue = cameraPropAttribute.HasAttribute(CameraPropertyAttribute.HASRANGE) ? dataStruct.ValueMin : null;
            MaxValue = cameraPropAttribute.HasAttribute(CameraPropertyAttribute.HASRANGE) ? dataStruct.ValueMax : null;
            DefaultValue = cameraPropAttribute.HasAttribute(CameraPropertyAttribute.HASDEFAULT) ? dataStruct.ValueDefault : null;

            Name = apiReference.GetPropertyName((int)PropertyID);

            Attributes = new AttributeFlags(cameraPropAttribute);

            int nEl = 1;
            if (dataStruct.NumberOfElements > 0)
                nEl = (int)apiReference.GetPropertyValue(dataStruct.NumberOfElements);

            PropertyIDValue = new int[nEl];
            PropertyIDValue[0] = (int)PropertyID;
            for (int i = 1; i < nEl; i++)
            {
                PropertyIDValue[i] = PropertyIDValue[i - 1] + dataStruct.ElementStep;
            }
        }

        public CameraProperty<double> ToTypedProperty()
        {
            return new CameraProperty<double>(apiReference, PropertyID, Step, MinValue, MaxValue, DefaultValue,
                                           Name, Attributes, PropertyIDValue, (x) => x);
        }

        protected double GetValueDouble(int index)
        {
            if (!Attributes.Readable) return double.NaN;

            if (PropertyIDValue.Length > index)

                return apiReference.GetPropertyValue(PropertyIDValue[index]);

            return apiReference.GetPropertyValue(PropertyIDValue[0]);
        }

        protected void SetValueDouble(int index, double value)
        {
            if (index > PropertyIDValue.Length)
                throw new Exception();

            apiReference.SetPropertyValue(PropertyIDValue[index], value);
        }
    }
}