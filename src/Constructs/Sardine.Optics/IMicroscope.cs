namespace Sardine.Optics
{
    public interface IMicroscope
    {
        public IOpticalCouplingInterface Sample { get; }
        public OpticalCoupling[] Couplings { get; }
    }


}
