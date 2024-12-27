using Sardine.Core;
using Sardine.Core.DataModel;

namespace ExampleSystem
{
    public class MySystem : Fleet
    {
        public Vessel<CameraService> CameraProvider { get; }
        public Vessel<Camera> ImagingCamera { get; }
        public Vessel<DataSaver> FrameSaver { get; }

        public MySystem()
        {
            string? cameraSN = Current.SettingsProvider.FetchSetting("CameraSettings", "CameraSN")?.Value;

            CameraProvider = Freighter.Freight(
                    builder: () => new CameraService()
                     );

            ImagingCamera = Freighter.Freight(
                    CameraProvider,
                    builder: (provider) => provider.GetCameras().Where((camera) => camera.SerialNumber == cameraSN ).FirstOrDefault()!,
                    initializer: (provider, camera) => camera.Start(),
                    invalidator: (provider, camera) => camera.Stop()
                            );

            FrameSaver = Freighter.Freight(() => new DataSaver("images"));

            ImagingCamera.AddSource(CameraFrameSource);
            FrameSaver.AddSink<CameraFrame>(FrameSink);
            ImagingCamera.SourceRate = 100; // polling rate to the camera
        }

        public static CameraFrame? CameraFrameSource(Camera cameraHandle, out bool hasMore)
        {
            CameraFrame? frame = cameraHandle.GetNextFrame();

            hasMore = (frame != null);

            return frame;
        }

        public static void FrameSink(DataSaver saverHandle, CameraFrame dataIn, MessageMetadata metadata)
        {
            string filename = $"{metadata.SenderName}_{metadata.SourceID}.bin";
            saverHandle.SaveData(filename, dataIn.Data);
        }
    }

}
