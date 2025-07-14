namespace Sardine.Utils.Waveforms
{
    public static class MathTransforms
    {

        public static double[] LinearInterpolation(double start, double end, int nPoints)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(nPoints, 2);

            double[] interpolationResult = new double[nPoints];
            double step = (end - start) / (nPoints - 1);
            interpolationResult[0] = start;
            for (int i = 1; i < nPoints; i++)
            {
                interpolationResult[i] = interpolationResult[i - 1] + step;
            }

            return interpolationResult;
        }
    }
}
