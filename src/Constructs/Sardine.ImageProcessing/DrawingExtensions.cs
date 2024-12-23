namespace Sardine.ImageProcessing
{
    public static class DrawingExtensions
    {
        public static int FindMinimaGray16(byte[] gray16Data)
        {
            ArgumentNullException.ThrowIfNull(gray16Data);

            int minValue = 65535;
            int intValue;
            for (int i = 0; i < gray16Data.Length / 2; i++)
            {
                intValue = gray16Data[i * 2] + gray16Data[i * 2 + 1] * 256;
                if (intValue < minValue)
                {
                    minValue = intValue;
                }
            }
            return minValue;
        }

        public static int FindMaximaGray16(byte[] gray16Data)
        {
            ArgumentNullException.ThrowIfNull(gray16Data);

            int maxValue = 0;
            int intValue;
            for (int i = 0; i < gray16Data.Length / 2; i++)
            {
                intValue = gray16Data[i * 2] + gray16Data[i * 2 + 1] * 256;
                if (intValue > maxValue)
                {
                    maxValue = intValue;
                }
            }
            return maxValue;
        }

        public static int From16BitArray(byte[] gray16Data, int index)
        {
            ArgumentNullException.ThrowIfNull(gray16Data);

            return gray16Data[index] + gray16Data[index + 1] * 256;
        }


#pragma warning disable CA1045 // Do not pass types by reference
        public static void Gray16ToGray8Compressor(in byte[]? inData, ref byte[] outData, int minVal = 0, int maxVal = 65535)
#pragma warning restore CA1045 // Do not pass types by reference
        {
            if (inData is null || outData is null)
            {
                return;
            }

            double modBase = (maxVal - minVal) / 256.0;


            for (int i = 0; i < outData.Length; i++)
            {
                outData[i] = (byte)Math.Max(0, Math.Min(255, (int)((inData[i * 2] + inData[i * 2 + 1] * 256 - minVal) / modBase)));
            }
        }

        public static void ApplyContrastGray8(in byte[]? inData, ref byte[] outData, int minVal = 0, int maxVal = 255)
        {
            if (inData is null || outData is null)
            {
                return;
            }

            double modBase = (maxVal - minVal) / 256.0;


            for (int i = 0; i < outData.Length; i++)
            {
                outData[i] = (byte)Math.Max(0, Math.Min(255, (inData[i] - minVal) / modBase));
            }
        }

        public static byte[] HSVtoRGB(double hue, double saturation = 1, double value = 1)
        {
            double red, green, blue;

            switch ((int)Math.Floor(hue * 6))
            {
                case 0:
                    red = value;
                    blue = value * (1 - saturation);
                    green = blue + hue * 6 * value * saturation;
                    break;
                case 1:
                    blue = value * (1 - saturation);
                    green = value;
                    red = green - (hue * 6 - 1) * value * saturation;
                    break;
                case 2:
                    green = value;
                    red = value * (1 - saturation);
                    blue = red + (hue * 6 - 2) * value * saturation;
                    break;
                case 3:
                    blue = value;
                    red = value * (1 - saturation);
                    green = blue - (hue * 6 - 3) * value * saturation;
                    break;
                case 4:
                    blue = value;
                    green = value * (1 - saturation);
                    red = green + (hue * 6 - 4) * value * saturation;
                    break;
                case 5:
                    green = value * (1 - saturation);
                    red = value;
                    blue = red - (hue * 6 - 5) * value * saturation;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(hue));
            }

            return [(byte)Math.Min(255, Math.Floor(red * 256)),
                               (byte)Math.Min(255, Math.Floor(green * 256)),
                               (byte)Math.Min(255, Math.Floor(blue * 256)) ];
        }



    }
}
