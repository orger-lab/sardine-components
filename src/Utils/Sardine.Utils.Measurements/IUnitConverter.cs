namespace Sardine.Utils.Measurements
{
    public interface IUnitConverter<Tin, Tout> where Tin : MeasuringUnit, new() where Tout : MeasuringUnit, new()
    {
        public Measure<Tout> Convert(Measure<Tin> value);
        public Measure<Tin> ConvertBack(Measure<Tout> value);

        public IUnitConverter<Tin, TSOut> Append<TSOut>(IUnitConverter<Tout, TSOut> otherConverter) where TSOut : MeasuringUnit, new()
        {
            return new UnitConverter<Tin, TSOut>(x => otherConverter.Convert(Convert(x)),
                                                x => ConvertBack(otherConverter.ConvertBack(x)));
        }

        public double IterativeConversionEstimator(double valueOut)
        {
            double exitCon = 0.0001;
            double multiplier = 0.5;

            double x = valueOut;
            double xM = valueOut * (1 + multiplier);
            double xm = valueOut * (1 - (multiplier / 2));

            double err, errM;


            while (true)
            {
                err = Convert(x) - valueOut;
                errM = Convert(xM) - valueOut;

                if (Math.Abs(err) < exitCon)
                {
                    break;
                }
                x = (err > 0 && errM < err) || (err < 0 && errM > err) ? (x + xM) / 2 : (x + xm) / 2;

                xM = x * (1 + multiplier);
                xm = x * (1 - (multiplier / 2));
            }


            return x;
        }

    }
}
