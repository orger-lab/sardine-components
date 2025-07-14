namespace Sardine.Optics
{
    public sealed class RadiationSpectrum
    {
        public double[][] Range { get; }
        public double[] Power { get; set; }

        public Func<double, double>[] PowerCurve { get; }

        public double GetSingleWavelength() => Range[0][0];

        public bool HasSingleWavelength { get; } = false;

        public double GetPower(double wavelength)
        {
            if (HasSingleWavelength)
            {
                return Power[0];
            }

            double power = 0;

            for (int i = 0; i < Range.Length; i++)
            {
                double[] spectrumRange = Range[i];
                if (spectrumRange.Length == 2)
                {
                    if (spectrumRange[0] <= wavelength && spectrumRange[1] >= wavelength)
                    {
                        power += PowerCurve[i](wavelength) * Power[i];
                    }
                }
                else
                {
                    if (spectrumRange[0] == wavelength)
                    {
                        power += Power[i];
                    }
                }
            }

            return power;
        }

        public static implicit operator RadiationSpectrum(double wavelength)
        {
            return new(wavelength);
        }

        public static implicit operator double(RadiationSpectrum spectrum)
        {
            return spectrum.GetSingleWavelength();
        }

        public static RadiationSpectrum None => new(double.NaN);

        public RadiationSpectrum(double wavelength) : this(wavelength, 1) { }
        public RadiationSpectrum(double wavelength, double power) : this(new double[1][] { new double[1] { wavelength } }, new double[1] { power }) { }

        public RadiationSpectrum(double[][] range, double[] power, Func<double, double>[] powerCurve) : this(range, power)
        {
            if (powerCurve.Length != Range.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(powerCurve));
            }

            PowerCurve = powerCurve;
        }

        public RadiationSpectrum(double[][] range, double[] power)
        {
            if (range.Length != power.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(power));
            }

            foreach (double[] r in range)
            {
                if (r.Length is > 2 or < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(range));
                }
            }

            Range = range;
            Power = power;
            PowerCurve = new Func<double, double>[power.Length];
            for (int i = 0; i < Power.Length; i++)
            {
                PowerCurve[i] = (x) => x;
            }

            if (range.Length == 1 && range[0].Length == 1)
            {
                HasSingleWavelength = true;
            }
        }

        public static byte[] WavelengthToRGB(double wavelength)
        {
            double Gamma = 0.80;
            double red = 0, green = 0, blue = 0;
            wavelength *= 1e9;
            if (wavelength < 380)
            {
                red = -(380 - 440) / (440 - 380);
                green = 0.0;
                blue = 1.0;
            }
            if (wavelength is >= 380 and < 440)
            {
                red = -(wavelength - 440) / (440 - 380);
                green = 0.0;
                blue = 1.0;
            }
            else if (wavelength is >= 440 and < 490)
            {
                red = 0.0;
                green = (wavelength - 440) / (490 - 440);
                blue = 1.0;
            }
            else if (wavelength is >= 490 and < 510)
            {
                red = 0.0;
                green = 1.0;
                blue = -(wavelength - 510) / (510 - 490);
            }
            else if (wavelength is >= 510 and < 580)
            {
                red = (wavelength - 510) / (580 - 510);
                green = 1.0;
                blue = 0.0;
            }
            else if (wavelength is >= 580 and < 645)
            {
                red = 1.0;
                green = -(wavelength - 645) / (645 - 580);
                blue = 0.0;
            }
            else if (wavelength >= 645)
            {
                red = 1.0;
                green = 0.0;
                blue = 0.0;
            }

            double factor;
            // Let the intensity fall off near the vision limits
            if (wavelength is >= 380 and < 420)
            {
                factor = 0.3 + (0.7 * (wavelength - 380) / (420 - 380));
            }
            else
            {
                factor = wavelength is >= 420 and < 701 ? 1.0 : wavelength is >= 701 and < 781 ? 0.3 + (0.7 * (780 - wavelength) / (780 - 700)) : 0.0;
            }
;
            if (red != 0)
            {
                red = Math.Pow(red * factor, Gamma);
            }
            if (green != 0)
            {
                green = Math.Pow(green * factor, Gamma);
            }
            if (blue != 0)
            {
                blue = Math.Pow(blue * factor, Gamma);
            }

            return new byte[3]{(byte)Math.Min(255, Math.Floor(red * 256)),
                               (byte)Math.Min(255, Math.Floor(green * 256)),
                               (byte)Math.Min(255, Math.Floor(blue * 256)) };
        }

    }


}
