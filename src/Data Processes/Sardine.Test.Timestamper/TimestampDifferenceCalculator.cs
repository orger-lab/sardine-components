using Sardine.Core.DataModel;
using Sardine.Recording.Data.Text;

namespace Sardine.Test.Timestamper
{
    public class TimestampDifferenceCalculator
    {
        public TimestampDifferenceCalculator() { }
        
        public static TimestampDifference Transform<T>(TimestampDifferenceCalculator tdCalculator, TimestampedContainer<TimestampedContainer<T>> dataIn, MessageMetadata metadata) where T : ITimestampedObject, IValueContainer
        {
            return new TimestampDifference(metadata.SourceID, dataIn.Payload.Payload.Timestamp, dataIn.Timestamp - dataIn.Payload.Timestamp, dataIn.Value);
        }
    }

    public interface ITimestampedObject
    {
        public DateTime Timestamp { get; }
    }

    public class TimestampDifference : ITextWritable
    {
        public bool IsAlive => true;
        public int Value { get; }
        public long TimestampID { get; }
        public DateTime Start { get; }
        public TimeSpan Difference { get; }
        public TimestampDifference(long id, DateTime start, TimeSpan span, int value)
        {
            Difference = span;
            Start = start;
            TimestampID = id;
            Value = value;
        }

        public string WriteData()
        {
            return $"{TimestampID} {Start.Ticks} {Difference.Ticks} {Value}";
        }

        public string WriteHeader()
        {
            return "TimestampID StartTicks DifferenceTicks Value";
        }
    }
}
