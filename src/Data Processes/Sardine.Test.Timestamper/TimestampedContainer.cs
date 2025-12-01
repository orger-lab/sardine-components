using Sardine.Core.DataModel;

namespace Sardine.Test.Timestamper
{

    public interface IValueContainer
    {
        public int Value { get; set; }
    }

    public class TimestampedContainer<T> : ITimestampedObject, IValueContainer where T: IValueContainer
    {
        public DateTime Timestamp { get; }

        public int Value { get => Payload.Value; set => Payload.Value = value; }
        public T Payload { get; }
        public TimestampedContainer(T payload)
        {
            Payload = payload;
            Timestamp = DateTime.UtcNow;
        }
    }

    public class TimestampAppender
    {

        public static TimestampedContainer<T> Transform<T>(TimestampAppender appender, T payload, MessageMetadata metadata) where T : IValueContainer
        {
            return new TimestampedContainer<T>(payload);
        }
    }

    
}
