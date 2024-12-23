using System.Collections;

namespace Sardine.Utils.Measurements
{
    public class SampleSequence<TUnit> : IReadOnlyCollection<Measure<TUnit>>, IEquatable<SampleSequence<TUnit>> where TUnit : MeasuringUnit, new()
    {
        private readonly Measure<TUnit>[] valueContainer;

        public static MeasuringUnit Unit { get; } = new TUnit();

        public bool IsSingleValue => ((IReadOnlyCollection<Measure<TUnit>>)valueContainer).Count == 1;
        public int Count => ((IReadOnlyCollection<Measure<TUnit>>)valueContainer).Count;

        public SampleSequence<TUnit> Map(Func<double, double> mapFunction)
        {
            double[] mappedValues = new double[valueContainer.Length];
            for (int i = 0; i < valueContainer.Length; i++)
            {
                mappedValues[i] = mapFunction(valueContainer[i]);
            }
            return new SampleSequence<TUnit>(mappedValues);
        }

        public SampleSequence<TOutUnit> Convert<TOutUnit>(IUnitConverter<TUnit, TOutUnit> unitConverter) where TOutUnit : MeasuringUnit, new() => new(valueContainer.Select(unitConverter.Convert));//, SampleRate, Delay, RepeatSamples);

        public Measure<TUnit> this[int index] => valueContainer[index];

        public static SampleSequence<TUnit> Empty => new(Array.Empty<double>());

        public SampleSequence(IEnumerable<IMeasure> values)
        {
            IEnumerable<Measure<TUnit>?> convertedValues = values.Select(Measure<TUnit>.TryConvertValue);
            if (convertedValues.Any(x => x is null))
            {
                throw new ArgumentException(null, nameof(values));
            }

            valueContainer = convertedValues.Select(x => x!).ToArray();

        }

        public SampleSequence(IMeasure value)
                         : this(new[] { value }) { }

        public SampleSequence(IEnumerable<double> values)
                          : this(values.Select(x => new Measure<TUnit>(x))) { }

        public SampleSequence(double value)
                         : this(new[] { value }) { }

        public static implicit operator double(SampleSequence<TUnit> staticValueCollection)
        {
            return staticValueCollection.valueContainer[0];
        }

        public static explicit operator SampleSequence<TUnit>(double value)
        {
            return new SampleSequence<TUnit>(value);
        }


        public IEnumerator<Measure<TUnit>> GetEnumerator() => ((IEnumerable<Measure<TUnit>>)valueContainer).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => valueContainer.GetEnumerator();
        public double[] GetValues()
        {
            double[] newValueSet = new double[Count];
            for (int i = 0; i < Count; i++)
            {
                newValueSet[i] = valueContainer[i];
            }

            return newValueSet;
        }

        public bool Equals(SampleSequence<TUnit>? other) => other is not null && GetHashCode() == other.GetHashCode();

        public override bool Equals(object? obj) => Equals(obj as SampleSequence<TUnit>);

        public static bool operator <(SampleSequence<TUnit> sc1, SampleSequence<TUnit> sc2)
        {
            return sc1 is not null && sc2 is not null && sc1.Zip(sc2).All((x) => x.First < x.Second);
        }

        public static bool operator >(SampleSequence<TUnit> sc1, SampleSequence<TUnit> sc2)
        {
            return sc1 is not null && sc2 is not null && sc1.Zip(sc2).All((x) => x.First > x.Second);
        }

        public static bool operator <=(SampleSequence<TUnit> sc1, SampleSequence<TUnit> sc2)
        {
            return sc1 is not null && sc2 is not null && sc1.Zip(sc2).All((x) => x.First <= x.Second);
        }

        public static bool operator >=(SampleSequence<TUnit> sc1, SampleSequence<TUnit> sc2)
        {
            return sc1 is not null && sc2 is not null && sc1.Zip(sc2).All((x) => x.First >= x.Second);
        }

        public override int GetHashCode() => valueContainer.GetHashCode();
    }
}
