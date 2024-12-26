namespace ExampleSystem
{
    public class CameraService
    {
        public CameraService() { }

        public List<Camera> GetCameras()
        {
            return [new Camera("SN_Test", 10, 400, 400)];
        }
    }

}
