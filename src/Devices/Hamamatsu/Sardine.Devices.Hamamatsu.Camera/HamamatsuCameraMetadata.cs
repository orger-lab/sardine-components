using Sardine.Devices.Hamamatsu.Camera.API;


namespace Sardine.Devices.Hamamatsu.Camera
{
    public class HamamatsuCameraMetadata
    {
        internal readonly IHamamatsuAPIWrapper api;
        public int ID { get; }
        public string Bus { get; }
        public int CameraID { get; }
        public string Vendor { get; }
        public string Model { get; }
        public string CameraVersion { get; }
        public string DriverVersion { get; }
        public string ModuleVersion { get; }
        public string DCamAPIVersion { get; }
        public string CameraSeriesName { get; }

        internal HamamatsuCameraMetadata(IHamamatsuAPIWrapper api, int id, string bus, string cameraID, string vendor, string model,
                                       string cVersion, string dVersion, string mVersion, string apiVersion, string seriesName)
        {
            this.api = api;
            ID = id;
            Bus = bus;
            CameraID = Convert.ToInt32(cameraID.Split(' ')[1]);
            Vendor = vendor;
            Model = model;
            CameraVersion = cVersion;
            DriverVersion = dVersion;
            ModuleVersion = mVersion;
            DCamAPIVersion = apiVersion;
            CameraSeriesName = seriesName;
        }
    }
}
