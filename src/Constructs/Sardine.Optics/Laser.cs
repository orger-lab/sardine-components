namespace Sardine.Optics
{
    public abstract class Laser
    {
        public RadiationSpectrum Wavelength { get; protected set; } = RadiationSpectrum.None;
        public double BeamWidth { get; protected set; }
        public virtual double Power { get; set; }
        public double MaximumPower { get; protected set; }
        public double Magnification { get; protected set; }

        public abstract bool SetPower(double power);
        public abstract bool Start();
        public abstract void Stop();
    }
}
