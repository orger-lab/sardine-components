namespace Sardine.Devices.Hamamatsu.Camera.API
{
    public abstract partial class CameraProperty
    {
        public struct AttributeFlags
        {
            public bool Readable { get; }
            public bool Writeable { get; }
            public bool Autorounding { get; }
            public bool InconsistentSteps { get; }
            public bool Volatile { get; }
            public bool Datastream { get; }
            public bool AccessibleInReadyStatus { get; }
            public bool AccessibleInBusyStatus { get; }
            public bool HasValueText { get; }
            public bool IsDouble { get; }
            public bool IsModal { get; }

            internal AttributeFlags(CameraPropertyAttribute attributeValue)
            {
                HasValueText = attributeValue.HasAttribute(CameraPropertyAttribute.HASVALUETEXT);
                Readable = attributeValue.HasAttribute(CameraPropertyAttribute.READABLE);
                Writeable = attributeValue.HasAttribute(CameraPropertyAttribute.WRITABLE);
                Autorounding = attributeValue.HasAttribute(CameraPropertyAttribute.AUTOROUNDING);
                InconsistentSteps = attributeValue.HasAttribute(CameraPropertyAttribute.STEPPING_INCONSISTENT);
                Volatile = attributeValue.HasAttribute(CameraPropertyAttribute.VOLATILE);
                Datastream = attributeValue.HasAttribute(CameraPropertyAttribute.DATASTREAM);
                AccessibleInReadyStatus = attributeValue.HasAttribute(CameraPropertyAttribute.ACCESSREADY);
                AccessibleInBusyStatus = attributeValue.HasAttribute(CameraPropertyAttribute.ACCESSBUSY);
                IsDouble = attributeValue.IsType(CameraPropertyAttribute.TYPE_REAL);
                IsModal = attributeValue.IsType(CameraPropertyAttribute.TYPE_MODE);
            }
        }
    }
}