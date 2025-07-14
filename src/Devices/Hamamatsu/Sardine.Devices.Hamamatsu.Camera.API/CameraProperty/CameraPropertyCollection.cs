using System.Collections.Generic;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    public class CameraPropertyCollection
    {
        Dictionary<CameraPropertyID, CameraProperty> Properties { get; }

        public CameraProperty<SensorMode>? SensorMode => (CameraProperty<SensorMode>?)Properties.GetValueOrDefault(CameraPropertyID.SensorMode);
        public CameraProperty<ReadoutSpeed>? ReadoutSpeed => (CameraProperty<ReadoutSpeed>?)Properties.GetValueOrDefault(CameraPropertyID.ReadoutSpeed);
        public CameraProperty<ReadoutDirection>? ReadoutDirection => (CameraProperty<ReadoutDirection>?)Properties.GetValueOrDefault(CameraPropertyID.ReadoutDirection);
        public CameraProperty<ColorType>? ColorType => (CameraProperty<ColorType>?)Properties.GetValueOrDefault(CameraPropertyID.ColorType);
        public CameraProperty<Mode>? BitsPerChannel => (CameraProperty<Mode>?)Properties.GetValueOrDefault(CameraPropertyID.BitsPerChannel)?.ToTypedProperty();
        public CameraProperty<TriggerSource>? TriggerSource => (CameraProperty<TriggerSource>?)Properties.GetValueOrDefault(CameraPropertyID.TriggerSource);
        public CameraProperty<TriggerMode>? TriggerMode => (CameraProperty<TriggerMode>?)Properties.GetValueOrDefault(CameraPropertyID.TriggerMode);
        public CameraProperty<TriggerActive>? TriggerActive => (CameraProperty<TriggerActive>?)Properties.GetValueOrDefault(CameraPropertyID.TriggerActive);
        public CameraProperty<TriggerGlobalExposure>? TriggerGlobalExposure => (CameraProperty<TriggerGlobalExposure>?)Properties.GetValueOrDefault(CameraPropertyID.TriggerGlobalExposure);
        public CameraProperty<TriggerPolarity>? TriggerPolarity => (CameraProperty<TriggerPolarity>?)Properties.GetValueOrDefault(CameraPropertyID.TriggerPolarity);
        public CameraProperty<TriggerConnector>? TriggerConnector => (CameraProperty<TriggerConnector>?)Properties.GetValueOrDefault(CameraPropertyID.TriggerConnector);
        public CameraProperty<int>? TriggerTimes => (CameraProperty<int>?)Properties.GetValueOrDefault(CameraPropertyID.TriggerTimes)?.ToTypedProperty();
        public CameraProperty<double>? TriggerDelay => Properties.GetValueOrDefault(CameraPropertyID.TriggerDelay)?.ToTypedProperty();
        public CameraProperty<SensorCoolerStatus>? SensorCoolerStatus => (CameraProperty<SensorCoolerStatus>?)Properties.GetValueOrDefault(CameraPropertyID.SensorCoolerStatus);
        public CameraProperty<double>? ExposureTime => Properties.GetValueOrDefault(CameraPropertyID.ExposureTime)?.ToTypedProperty();
        public CameraProperty<DefectCorrectMode>? DefectCorrectMode => (CameraProperty<DefectCorrectMode>?)Properties.GetValueOrDefault(CameraPropertyID.DefectCorrectMode);
        public CameraProperty<SubarraySize>? Binning => (CameraProperty<SubarraySize>?)Properties.GetValueOrDefault(CameraPropertyID.Binning);
        public CameraProperty<int>? SubarrayHorizontalPosition => (CameraProperty<int>?)Properties.GetValueOrDefault(CameraPropertyID.SubarrayHorizontalPosition)?.ToTypedProperty();
        public CameraProperty<int>? SubarrayHorizontalSize => (CameraProperty<int>?)Properties.GetValueOrDefault(CameraPropertyID.SubarrayHorizontalSize)?.ToTypedProperty();
        public CameraProperty<int>? SubarrayVerticalPosition => (CameraProperty<int>?)Properties.GetValueOrDefault(CameraPropertyID.SubarrayVerticalPosition)?.ToTypedProperty();
        public CameraProperty<int>? SubarrayVerticalSize => (CameraProperty<int>?)Properties.GetValueOrDefault(CameraPropertyID.SubarrayVerticalSize)?.ToTypedProperty();
        public CameraProperty<Mode>? SubarrayMode => (CameraProperty<Mode>?)Properties.GetValueOrDefault(CameraPropertyID.SubarrayMode)?.ToTypedProperty();
        public CameraProperty<Mode>? TimingReadoutTime => (CameraProperty<Mode>?)Properties.GetValueOrDefault(CameraPropertyID.TimingReadoutTime)?.ToTypedProperty();
        public CameraProperty<double>? TimingCyclicTriggerPeriod => Properties.GetValueOrDefault(CameraPropertyID.TimingCyclicTriggerPeriod)?.ToTypedProperty();
        public CameraProperty<double>? TimingMinTriggerBlanking => Properties.GetValueOrDefault(CameraPropertyID.TimingMinTriggerBlanking)?.ToTypedProperty();
        public CameraProperty<double>? TimingMinTriggerInterval => Properties.GetValueOrDefault(CameraPropertyID.TimingMinTriggerInterval)?.ToTypedProperty();
        public CameraProperty<double>? TimingGlobalExposureDelay => Properties.GetValueOrDefault(CameraPropertyID.TimingGlobalExposureDelay)?.ToTypedProperty();
        public CameraProperty<TimingExposure>? TimingExposure => (CameraProperty<TimingExposure>?)Properties.GetValueOrDefault(CameraPropertyID.TimingExposure);
        public CameraProperty<Mode>? TimingInvalidExposurePeriod => (CameraProperty<Mode>?)Properties.GetValueOrDefault(CameraPropertyID.TimingInvalidExposurePeriod)?.ToTypedProperty();
        public CameraProperty<double>? InternalFramerate => Properties.GetValueOrDefault(CameraPropertyID.InternalFramerate)?.ToTypedProperty();
        public CameraProperty<double>? InternalFrameInterval => Properties.GetValueOrDefault(CameraPropertyID.InternalFrameInterval)?.ToTypedProperty();
        public CameraProperty<double>? InternalLineSpeed => Properties.GetValueOrDefault(CameraPropertyID.InternalLineSpeed)?.ToTypedProperty();
        public CameraProperty<double>? InternalLineInterval => Properties.GetValueOrDefault(CameraPropertyID.InternalLineInterval)?.ToTypedProperty();
        public CameraProperty<int>? ImageWidth => (CameraProperty<int>?)Properties.GetValueOrDefault(CameraPropertyID.ImageWidth)?.ToTypedProperty();
        public CameraProperty<int>? ImageHeight => (CameraProperty<int>?)Properties.GetValueOrDefault(CameraPropertyID.ImageHeight)?.ToTypedProperty();
        public CameraProperty<int>? ImageRowBytes => (CameraProperty<int>?)Properties.GetValueOrDefault(CameraPropertyID.ImageRowBytes)?.ToTypedProperty();
        public CameraProperty<int>? ImageFrameBytes => (CameraProperty<int>?)Properties.GetValueOrDefault(CameraPropertyID.ImageFrameBytes)?.ToTypedProperty();
        public CameraProperty<int>? ImageTopOffsetBytes => (CameraProperty<int>?)Properties.GetValueOrDefault(CameraPropertyID.ImageTopOffsetBytes)?.ToTypedProperty();
        public CameraProperty<PixelType>? ImagePixelType => (CameraProperty<PixelType>?)Properties.GetValueOrDefault(CameraPropertyID.ImagePixelType);
        public CameraProperty<int>? BufferRowBytes => (CameraProperty<int>?)Properties.GetValueOrDefault(CameraPropertyID.BufferRowBytes)?.ToTypedProperty();
        public CameraProperty<int>? BufferFrameBytes => (CameraProperty<int>?)Properties.GetValueOrDefault(CameraPropertyID.BufferFrameBytes)?.ToTypedProperty();
        public CameraProperty<int>? BufferTopOffsetBytes => (CameraProperty<int>?)Properties.GetValueOrDefault(CameraPropertyID.BufferTopOffsetBytes)?.ToTypedProperty();
        public CameraProperty<PixelType>? BufferPixelType => (CameraProperty<PixelType>?)Properties.GetValueOrDefault(CameraPropertyID.BufferPixelType);
        public CameraProperty<Mode>? RecordFixedBytesPerFile => (CameraProperty<Mode>?)Properties.GetValueOrDefault(CameraPropertyID.RecordFixedBytesPerFile)?.ToTypedProperty();
        public CameraProperty<int>? RecordFixedBytesPerSession => (CameraProperty<int>?)Properties.GetValueOrDefault(CameraPropertyID.RecordFixedBytesPerSession)?.ToTypedProperty();
        public CameraProperty<int>? RecordFixedBytesPerFrame => (CameraProperty<int>?)Properties.GetValueOrDefault(CameraPropertyID.RecordFixedBytesPerFrame)?.ToTypedProperty();
        public CameraProperty<int>? NumberOfOutputTriggerConnector => (CameraProperty<int>?)Properties.GetValueOrDefault(CameraPropertyID.NumberOfOutputTriggerConnector)?.ToTypedProperty();
        public CameraProperty<OutputTriggerSource>? OutputTriggerSource => (CameraProperty<OutputTriggerSource>?)Properties.GetValueOrDefault(CameraPropertyID.OutputTriggerSource);
        public CameraProperty<OutputTriggerPolarity>? OutputTriggerPolarity => (CameraProperty<OutputTriggerPolarity>?)Properties.GetValueOrDefault(CameraPropertyID.OutputTriggerPolarity);
        public CameraProperty<OutputTriggerActiveRegion>? OutputTriggerActiveRegion => (CameraProperty<OutputTriggerActiveRegion>?)Properties.GetValueOrDefault(CameraPropertyID.OutputTriggerActive);
        public CameraProperty<double>? OutputTriggerDelay => Properties.GetValueOrDefault(CameraPropertyID.OutputTriggerDelay)?.ToTypedProperty();
        public CameraProperty<double>? OutputTriggerPeriod => Properties.GetValueOrDefault(CameraPropertyID.OutputTriggerPeriod)?.ToTypedProperty();
        public CameraProperty<OutputTriggerKind>? OutputTriggerKind => (CameraProperty<OutputTriggerKind>?)Properties.GetValueOrDefault(CameraPropertyID.OutputTriggerKind);
        public CameraProperty<Mode>? OutputTriggerPreHSyncCount => (CameraProperty<Mode>?)Properties.GetValueOrDefault(CameraPropertyID.OutputTriggerPreHSyncCount)?.ToTypedProperty();
        public CameraProperty<SystemAlive>? SystemAlive => (CameraProperty<SystemAlive>?)Properties.GetValueOrDefault(CameraPropertyID.SystemAlive);
        public CameraProperty<Mode>? ConversionFactorCoeff => (CameraProperty<Mode>?)Properties.GetValueOrDefault(CameraPropertyID.ConversionFactorCoeff)?.ToTypedProperty();
        public CameraProperty<double>? ConversionFactorOffset => Properties.GetValueOrDefault(CameraPropertyID.ConversionFactorOffset)?.ToTypedProperty();
        public CameraProperty<double>? ImageDetectorPixelWidth => Properties.GetValueOrDefault(CameraPropertyID.ImageDetectorPixelWidth)?.ToTypedProperty();
        public CameraProperty<double>? ImageDetectorPixelHeight => Properties.GetValueOrDefault(CameraPropertyID.ImageDetectorPixelHeight)?.ToTypedProperty();
        public CameraProperty<double>? ImageDetectorPixelNumHorizontal => Properties.GetValueOrDefault(CameraPropertyID.ImageDetectorPixelNumHorizontal)?.ToTypedProperty();
        public CameraProperty<int>? ImageDetectorPixelNumVertical => (CameraProperty<int>?)Properties.GetValueOrDefault(CameraPropertyID.ImageDetectorPixelNumVertical)?.ToTypedProperty();
        public CameraProperty<TimestampProducer>? TimestampProducer => (CameraProperty<TimestampProducer>?)Properties.GetValueOrDefault(CameraPropertyID.TimestampProducer);
        public CameraProperty<FramestampProducer>? FramestampProducer => (CameraProperty<FramestampProducer>?)Properties.GetValueOrDefault(CameraPropertyID.FramestampProducer);
        public CameraProperty<InternalTriggerHandling>? InternalTriggerHandling => (CameraProperty<InternalTriggerHandling>?)Properties.GetValueOrDefault(CameraPropertyID.InternalTriggerHandling);
        public CameraProperty<ExposureTimeControl>? ExposureTimeControl => (CameraProperty<ExposureTimeControl>?)Properties.GetValueOrDefault(CameraPropertyID.ExposureTimeControl);
        public CameraProperty<Mode>? FramebundleMode => (CameraProperty<Mode>?)Properties.GetValueOrDefault(CameraPropertyID.FramebundleMode)?.ToTypedProperty();
        public CameraProperty<Mode>? FramebundleNumber => (CameraProperty<Mode>?)Properties.GetValueOrDefault(CameraPropertyID.FramebundleNumber)?.ToTypedProperty();
        public CameraProperty<int>? FramebundleRowBytes => (CameraProperty<int>?)Properties.GetValueOrDefault(CameraPropertyID.FramebundleRowBytes)?.ToTypedProperty();
        public CameraProperty<int>? FramebundleFramestepBytes => (CameraProperty<int>?)Properties.GetValueOrDefault(CameraPropertyID.FramebundleFramestepBytes)?.ToTypedProperty();
        public CameraProperty<Mode>? ImageCameraStamp => (CameraProperty<Mode>?)Properties.GetValueOrDefault(CameraPropertyID.ImageCameraStamp)?.ToTypedProperty();
        public CameraProperty<int>? OutputTriggerChannelSync => (CameraProperty<int>?)Properties.GetValueOrDefault(CameraPropertyID.OutputTriggerChannelSync)?.ToTypedProperty();
        public CameraProperty<OutputTriggerProgrammableStart>? OutputTriggerProgrammableStart => (CameraProperty<OutputTriggerProgrammableStart>?)Properties.GetValueOrDefault(CameraPropertyID.OutputTriggerProgrammableStart);
        public CameraProperty<double>? NumberOfViews => Properties.GetValueOrDefault(CameraPropertyID.NumberOfViews)?.ToTypedProperty();

        public CameraPropertyCollection(Dictionary<CameraPropertyID, CameraProperty> properties)
        {
            Properties = properties;
        }
    }
}