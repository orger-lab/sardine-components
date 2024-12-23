namespace Sardine.Utils.Measurements
{
    public class UnitConverter<TI, TO>(Func<Measure<TI>, Measure<TO>> converter, Func<Measure<TO>, Measure<TI>> backConverter) : IUnitConverter<TI, TO> where TI : MeasuringUnit, new() where TO : MeasuringUnit, new()
    {
        public Measure<TO> Convert(Measure<TI> value) => converter(value);
        public Measure<TI> ConvertBack(Measure<TO> value) => backConverter(value);

        private readonly Func<Measure<TI>, Measure<TO>> converter = converter;
        private readonly Func<Measure<TO>, Measure<TI>> backConverter = backConverter;
    }
}
