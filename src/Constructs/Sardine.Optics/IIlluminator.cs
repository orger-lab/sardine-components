namespace Sardine.Optics
{
    public interface IAxialIlluminator
    {
        public RadiationSpectrum Wavelength { get; }
        public double NA { get; }
        public double Magnification { get; }
    }
}