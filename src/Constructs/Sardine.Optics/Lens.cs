using Sardine.Utils.Measurements;
using Sardine.Utils.Measurements.Angular;
using Sardine.Utils.Measurements.Size;

namespace Sardine.Optics
{
    public class Lens
    {
        private const double AIRY1 = 1.21966989;
        public string Name { get; init; }
        public Measure<Meter> FocalLength { get; }
        public Measure<Meter> Pupil { get; }
        public double NA { get; }
        public Measure<Meter> WorkingDistance { get; }
        public RefractiveIndex StandardMedium { get; }

        public Measure<Radian> MaximumAngle => Math.Asin(NA / StandardMedium);
        public double FNumber => FocalLength / Pupil;
        public Measure<Meter> ResolvingPower(double wavelength, RefractiveIndex? medium = null) => AIRY1 * wavelength / ((medium ?? StandardMedium) * NA / StandardMedium);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f">Focal length in meters</param>
        /// <param name="NA">Numerical aperture</param>
        /// <param name="pupil">Pupil size in meters</param>
        /// <param name="wd">Working distance</param>
        /// <param name="medium">Default working medium of the lens</param>
        /// <param name="name">Name of the lens</param>
        /// <exception cref="ArgumentException"></exception>
        public Lens(double f = double.NaN, double NA = double.NaN, double pupil = double.NaN, double wd = double.NaN, RefractiveIndex? medium = null, string? name = null)
        {
            Name = name ?? string.Empty;

            Pupil = pupil;
            this.NA = NA;
            FocalLength = f;
            StandardMedium = medium ?? 1;
            WorkingDistance = wd;

            if (double.IsNaN(Pupil))
            {
                Pupil = double.PositiveInfinity;
            }

            if (double.IsNaN(this.NA))
            {
                this.NA = StandardMedium * Math.Sin(Math.Atan(Pupil * 0.5 / FocalLength));
            }

            if (double.IsNaN(FocalLength))
            {
                FocalLength = 0.5 * Pupil / Math.Tan(MaximumAngle);
            }

            if (double.IsNaN(WorkingDistance))
            {
                WorkingDistance = FocalLength;
            }

            if (new double[3] { FocalLength, Pupil, this.NA }.Any(double.IsNaN))
            {
                throw new ArgumentException("Invalid input parametrics. Can't define lens.");
            }
        }

        public static implicit operator double(Lens lens)
        {
            return lens.FocalLength;
        }

        public override string ToString() => Name;
    }

    public class RefractiveIndex
    {
        public static RefractiveIndex Air => 1.0;
        public static RefractiveIndex Water => 1.333;
        public static RefractiveIndex Oil => 1.52;

        //public double RefractiveIndex { get; }
        private readonly double _refractiveIndex;

        public RefractiveIndex(double x)
        {
            _refractiveIndex = x;
        }

        public static implicit operator RefractiveIndex(double x)
        {
            return new(x);
        }

        public static implicit operator double(RefractiveIndex x)
        {
            return x._refractiveIndex;
        }
    }

    public class Telescope
    {
        public Lens Lens1 { get; }
        public Lens Lens2 { get; }

        public Telescope(Lens lens1, Lens lens2)
        {
            Lens1 = lens1;
            Lens2 = lens2;
        }
        public double Magnification => Lens2.FocalLength / Lens1.FocalLength;
    }
}
