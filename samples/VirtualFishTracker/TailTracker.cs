using Sardine.Core;
using Sardine.Core.Metadata;
using Sardine.Display.Skia;
using Sardine.ImageProcessing;
using Sardine.ImageProcessing.BGSubtraction;
using Sardine.ImageProcessing.BGSubtraction.OpenCV;
using Sardine.Recording.Data.Text;
using Sardine.Recording.Metadata;
using Sardine.Recording.Metadata.Zebrafish;
using Sardine.Recording.Stream.Binary;
using Sardine.Test.AVIReader;
using Sardine.Tracking.ZebraHRTailTracking;

namespace VirtualFishTracker
{
    public class TailTracker : Fleet
    {
        public Vessel<MPTailTracking> TrackingAlgorithm { get; }
        public Vessel<PositiveDiffWithMinBG> BGSubtractor { get; }
        public Vessel<SkiaSharpDisplay> BehaviourDisplay { get; }
        public Vessel<AVIReader> MockCameraService { get; }
        public Vessel<MoviePlayer> MockCamera { get; }
        public Vessel<TextMessageRecorder> OutputWriter { get; }
        public Vessel<SardineBinaryImageWriter> BinaryWriter { get; }
        public Vessel<ZebrafishExperimentMetadataController> ExperimentMetadata { get; }

        public TailTracker()
        {
            ExperimentMetadata = Freighter.Freight(builder: Current.Get<ZebrafishExperimentMetadataController>,
                                                   initializer: (metadataService) => metadataService.Metadata.ExperimentFolderBase = @"C:\");

            MockCameraService = Freighter.Freight(builder: () => new AVIReader());

            string filePath = @"output_converted_1_cropped.avi";
            MockCamera = Freighter.Freight(MockCameraService,
                                           (cameraService) => cameraService.GetPlayer(filePath));
            MockCamera.SourceRate = 350;
            MockCamera.AddSource(MoviePlayerSource.SourceFrame);

            BGSubtractor = Freighter.Freight(() => new PositiveDiffWithMinBG());
            BGSubtractor.AddTransformer<IImageFrame, IImageFrame>(BackgroundSubtracterTransformer.Transform, [MockCamera]);
            BGSubtractor.SetMetadataCollection(MetadataCollectionMethods.CollectBGSubtractor);

            TrackingAlgorithm = Freighter.Freight(() => new MPTailTracking());
            TrackingAlgorithm.AddTransformer<IImageFrame, MPTailTrackingResult>(MPTailTracking.Transform, [BGSubtractor]);
            TrackingAlgorithm.SetMetadataCollection(MetadataCollectionMethods.CollectTailTracker);

            BehaviourDisplay = SkiaSharpDisplay.GetVessel([MockCamera, TrackingAlgorithm], 30);

            OutputWriter = Freighter.Freight(() => new TextMessageRecorder());
            OutputWriter.AddSink<ITextWritable>(TextMessageRecorder.Sink);
            OutputWriter.RegisterSavePathAction<TextMessageRecorder, ZebrafishExperimentMetadataController, ZebrafishExperimentMetadata>((writer, path, filename) => { if (writer is not null) { writer.Path = path; writer.FileName = filename; } });

            BinaryWriter = Freighter.Freight(() => new SardineBinaryImageWriter());
            BinaryWriter.AddSink<IImageFrame>(SardineBinaryImageWriter.Sink, [MockCamera]);
            BinaryWriter.RegisterSavePathAction<SardineBinaryImageWriter, ZebrafishExperimentMetadataController, ZebrafishExperimentMetadata>((writer, path, filename) => { if (writer is not null) { writer.Path = path; writer.FileName = filename; } });
        }
    }


    public static class MetadataCollectionMethods
    {
        public static MetadataTable CollectTailTracker(MPTailTracking tracker)
        {
            var data = new Dictionary<string, MetadataObject>() {
                { "StartX" , new MetadataInteger((int)tracker.StartX)},
                { "EndX" , new MetadataInteger((int)tracker.EndX)},
                { "StartY" , new MetadataInteger((int)tracker.StartY)},
                { "EndY" , new MetadataInteger((int)tracker.EndY)},
                { "SearchThreshold" , new MetadataInteger(tracker.BodySearchThreshold)},
                };

            return new MetadataTable(data);
        }

        public static MetadataTable CollectBGSubtractor(PositiveDiffWithMinBG subtracter)
        {
            var data = new Dictionary<string, MetadataObject>() {
                { "KSize" , new MetadataInteger(subtracter.KSize)},
                };

            return new MetadataTable(data);
        }
    }
}
