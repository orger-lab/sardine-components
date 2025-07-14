namespace Sardine.Devices.NI.DAQ
{
    public sealed class DaqPFI : DaqInternalChannelID
    {
        public string DeviceName { get; }
        public int PFI { get; }

        public DaqPFI(string deviceName, int pfi):base($"/{deviceName}/PFI{pfi}", InternalChannelAccess.PFI)
        {
            PFI = pfi;
            DeviceName = deviceName;
        }
    }
}
