namespace Sardine.Optics
{
    public interface IOpticalCouplingInterface
    {
        public RefractiveIndex Medium { get; }
        public RadiationSpectrum EmissionRange { get; }
        public RadiationSpectrum ExcitationRange { get; }
    }
}
