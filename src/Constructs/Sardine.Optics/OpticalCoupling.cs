namespace Sardine.Optics
{
    public class OpticalCoupling
    {
        public double Roll { get; set; }
        public double Pitch { get; set; }
        public double Yaw { get; set; }
        public IAxialIlluminator ThrowingArm { get; set; }
        public IAxialIlluminator CatchingArm { get; set; }

        public OpticalCoupling(IAxialIlluminator throwingArm, IAxialIlluminator catchingArm, double roll = 0, double pitch = 0, double yaw = 0)
        {
            ThrowingArm = throwingArm;
            CatchingArm = catchingArm;
            Roll = roll;
            Pitch = pitch;
            Yaw = yaw;
        }

    }
}
