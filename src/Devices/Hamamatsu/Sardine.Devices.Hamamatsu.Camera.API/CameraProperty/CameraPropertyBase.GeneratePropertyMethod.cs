namespace Sardine.Devices.Hamamatsu.Camera.API
{
    public abstract partial class CameraProperty
    {
        internal static CameraProperty GenerateProperty(DCam camera, CameraPropertyID id, DCAMPropertyAttributeStruct dataStruct)
        {
            switch (id)
            {
                case CameraPropertyID.ReadoutSpeed:
                    return new CameraProperty<ReadoutSpeed>(camera, id, dataStruct, (x) => (ReadoutSpeed)x);
                case CameraPropertyID.TriggerSource:
                    return new CameraProperty<TriggerSource>(camera, id, dataStruct, (x) => (TriggerSource)x);
                case CameraPropertyID.TriggerActive:
                    return new CameraProperty<TriggerActive>(camera, id, dataStruct, (x) => (TriggerActive)x);
                case CameraPropertyID.TriggerMode:
                    return new CameraProperty<TriggerMode>(camera, id, dataStruct, (x) => (TriggerMode)x);
                case CameraPropertyID.TriggerPolarity:
                    return new CameraProperty<TriggerPolarity>(camera, id, dataStruct, (x) => (TriggerPolarity)x);
                case CameraPropertyID.TriggerConnector:
                    return new CameraProperty<TriggerConnector>(camera, id, dataStruct, (x) => (TriggerConnector)x);
                case CameraPropertyID.InternalTriggerHandling:
                    return new CameraProperty<InternalTriggerHandling>(camera, id, dataStruct, (x) => (InternalTriggerHandling)x);
                case CameraPropertyID.TriggerEnableActive:
                    return new CameraProperty<TriggerEnableActive>(camera, id, dataStruct, (x) => (TriggerEnableActive)x);
                case CameraPropertyID.TriggerEnablePolarity:
                    return new CameraProperty<TriggerEnablePolarity>(camera, id, dataStruct, (x) => (TriggerEnablePolarity)x);
                case CameraPropertyID.BusSpeed:
                    return new CameraProperty<ReadoutSpeed>(camera, id, dataStruct, (x) => (ReadoutSpeed)x);
                case CameraPropertyID.SyncReadoutSystemBlank:
                    return new CameraProperty<SyncReadoutSystemBlank>(camera, id, dataStruct, (x) => (SyncReadoutSystemBlank)x);
                case CameraPropertyID.OutputTriggerProgrammableStart:
                    return new CameraProperty<OutputTriggerProgrammableStart>(camera, id, dataStruct, (x) => (OutputTriggerProgrammableStart)x);
                case CameraPropertyID.OutputTriggerSource:
                    return new CameraProperty<OutputTriggerSource>(camera, id, dataStruct, (x) => (OutputTriggerSource)x);
                case CameraPropertyID.FramebundleMode:
                    return new CameraProperty<Mode>(camera, id, dataStruct, (x) => (Mode)x);
                case CameraPropertyID.SubarrayMode:
                    return new CameraProperty<Mode>(camera, id, dataStruct, (x) => (Mode)x);
                case CameraPropertyID.OutputTriggerPolarity:
                    return new CameraProperty<OutputTriggerPolarity>(camera, id, dataStruct, (x) => (OutputTriggerPolarity)x);
                case CameraPropertyID.OutputTriggerActive:
                    return new CameraProperty<OutputTriggerActiveRegion>(camera, id, dataStruct, (x) => (OutputTriggerActiveRegion)x);
                case CameraPropertyID.OutputTriggerKind:
                    return new CameraProperty<OutputTriggerKind>(camera, id, dataStruct, (x) => (OutputTriggerKind)x);
                case CameraPropertyID.OutputTriggerBaseSensor:
                    return new CameraProperty<OutputTriggerBaseSensor>(camera, id, dataStruct, (x) => (OutputTriggerBaseSensor)x);
                case CameraPropertyID.MasterPulseMode:
                    return new CameraProperty<MasterPulseMode>(camera, id, dataStruct, (x) => (MasterPulseMode)x);
                case CameraPropertyID.MasterPulseTriggerSource:
                    return new CameraProperty<MasterPulseTriggerSource>(camera, id, dataStruct, (x) => (MasterPulseTriggerSource)x);
                case CameraPropertyID.ExposureTimeControl:
                    return new CameraProperty<ExposureTimeControl>(camera, id, dataStruct, (x) => (ExposureTimeControl)x);
                case CameraPropertyID.TriggerFirstExposure:
                    return new CameraProperty<TriggerFirstExposure>(camera, id, dataStruct, (x) => (TriggerFirstExposure)x);
                case CameraPropertyID.TriggerGlobalExposure:
                    return new CameraProperty<TriggerGlobalExposure>(camera, id, dataStruct, (x) => (TriggerGlobalExposure)x);
                case CameraPropertyID.FirstTriggerBehaviour:
                    return new CameraProperty<FirstTriggerBehaviour>(camera, id, dataStruct, (x) => (FirstTriggerBehaviour)x);
                case CameraPropertyID.LightMode:
                    return new CameraProperty<LightMode>(camera, id, dataStruct, (x) => (LightMode)x);
                case CameraPropertyID.SensitivityMode:
                    return new CameraProperty<SensitivityMode>(camera, id, dataStruct, (x) => (SensitivityMode)x);
                case CameraPropertyID.EMGainWarningStatus:
                    return new CameraProperty<EMGainWarningStatus>(camera, id, dataStruct, (x) => (EMGainWarningStatus)x);
                case CameraPropertyID.SensorCooler:
                    return new CameraProperty<SensorCooler>(camera, id, dataStruct, (x) => (SensorCooler)x);
                case CameraPropertyID.SensorCoolerStatus:
                    return new CameraProperty<SensorCoolerStatus>(camera, id, dataStruct, (x) => (SensorCoolerStatus)x);
                case CameraPropertyID.SensorTemperatureStatus:
                    return new CameraProperty<SensorTemperatureStatus>(camera, id, dataStruct, (x) => (SensorTemperatureStatus)x);
                case CameraPropertyID.MechanicalShutter:
                    return new CameraProperty<MechanicalShutter>(camera, id, dataStruct, (x) => (MechanicalShutter)x);
                case CameraPropertyID.WhiteBalanceMode:
                    return new CameraProperty<WhiteBalanceMode>(camera, id, dataStruct, (x) => (WhiteBalanceMode)x);
                case CameraPropertyID.InterframeALUEnable:
                    return new CameraProperty<InterframeALUEnable>(camera, id, dataStruct, (x) => (InterframeALUEnable)x);
                case CameraPropertyID.ShadingCalibDataStatus:
                    return new CameraProperty<ShadingCalibDataStatus>(camera, id, dataStruct, (x) => (ShadingCalibDataStatus)x);
                case CameraPropertyID.ShadingCalibMethod:
                    return new CameraProperty<ShadingCalibMethod>(camera, id, dataStruct, (x) => (ShadingCalibMethod)x);
                case CameraPropertyID.DarkCalibTarget:
                    return new CameraProperty<DarkCalibTarget>(camera, id, dataStruct, (x) => (DarkCalibTarget)x);
                case CameraPropertyID.CaptureMode:
                    return new CameraProperty<CaptureMode>(camera, id, dataStruct, (x) => (CaptureMode)x);
                case CameraPropertyID.IntensityLUTMode:
                    return new CameraProperty<IntensityLUTMode>(camera, id, dataStruct, (x) => (IntensityLUTMode)x);
                case CameraPropertyID.TapGainCalibMethod:
                    return new CameraProperty<TapGainCalibMethod>(camera, id, dataStruct, (x) => (TapGainCalibMethod)x);
                case CameraPropertyID.ReadoutDirection:
                    return new CameraProperty<ReadoutDirection>(camera, id, dataStruct, (x) => (ReadoutDirection)x);
                case CameraPropertyID.ReadoutUnit:
                    return new CameraProperty<ReadoutUnit>(camera, id, dataStruct, (x) => (ReadoutUnit)x);
                case CameraPropertyID.ShutterMode:
                    return new CameraProperty<ShutterMode>(camera, id, dataStruct, (x) => (ShutterMode)x);
                case CameraPropertyID.SensorMode:
                    return new CameraProperty<SensorMode>(camera, id, dataStruct, (x) => (SensorMode)x);
                case CameraPropertyID.CCDMode:
                    return new CameraProperty<CCDMode>(camera, id, dataStruct, (x) => (CCDMode)x);
                case CameraPropertyID.CMOSMode:
                    return new CameraProperty<CMOSMode>(camera, id, dataStruct, (x) => (CMOSMode)x);
                case CameraPropertyID.OutputIntensity:
                    return new CameraProperty<OutputIntensity>(camera, id, dataStruct, (x) => (OutputIntensity)x);
                case CameraPropertyID.OutputDataOperation:
                    return new CameraProperty<OutputDataOperation>(camera, id, dataStruct, (x) => (OutputDataOperation)x);
                case CameraPropertyID.TestPatternKind:
                    return new CameraProperty<TestPatternKind>(camera, id, dataStruct, (x) => (TestPatternKind)x);
                case CameraPropertyID.Binning:
                    return new CameraProperty<SubarraySize>(camera, id, dataStruct, (x) => (SubarraySize)x);
                case CameraPropertyID.DigitalBinningMethod:
                    return new CameraProperty<DigitalBinningMethod>(camera, id, dataStruct, (x) => (DigitalBinningMethod)x);
                case CameraPropertyID.TimingExposure:
                    return new CameraProperty<TimingExposure>(camera, id, dataStruct, (x) => (TimingExposure)x);
                case CameraPropertyID.TimestampProducer:
                    return new CameraProperty<TimestampProducer>(camera, id, dataStruct, (x) => (TimestampProducer)x);
                case CameraPropertyID.FramestampProducer:
                    return new CameraProperty<FramestampProducer>(camera, id, dataStruct, (x) => (FramestampProducer)x);
                case CameraPropertyID.ColorType:
                    return new CameraProperty<ColorType>(camera, id, dataStruct, (x) => (ColorType)x);
                case CameraPropertyID.ImagePixelType:
                    return new CameraProperty<PixelType>(camera, id, dataStruct, (x) => (PixelType)x);
                case CameraPropertyID.BufferPixelType:
                    return new CameraProperty<PixelType>(camera, id, dataStruct, (x) => (PixelType)x);
                case CameraPropertyID.DefectCorrectMode:
                    return new CameraProperty<DefectCorrectMode>(camera, id, dataStruct, (x) => (DefectCorrectMode)x);
                case CameraPropertyID.HotPixelCorrectLevel:
                    return new CameraProperty<HotPixelCorrectLevel>(camera, id, dataStruct, (x) => (HotPixelCorrectLevel)x);
                case CameraPropertyID.DefectCorrectMethod:
                    return new CameraProperty<DefectCorrectMethod>(camera, id, dataStruct, (x) => (DefectCorrectMethod)x);
                case CameraPropertyID.CameraStatusIntensity:
                    return new CameraProperty<CameraStatusIntensity>(camera, id, dataStruct, (x) => (CameraStatusIntensity)x);
                case CameraPropertyID.CameraStatusInputTrigger:
                    return new CameraProperty<CameraStatusInputTrigger>(camera, id, dataStruct, (x) => (CameraStatusInputTrigger)x);
                case CameraPropertyID.CameraStatusCalibration:
                    return new CameraProperty<CameraStatusCalibration>(camera, id, dataStruct, (x) => (CameraStatusCalibration)x);
                case CameraPropertyID.SystemAlive:
                    return new CameraProperty<SystemAlive>(camera, id, dataStruct, (x) => (SystemAlive)x);

                default:
                    if (new CameraPropertyAttribute((uint)dataStruct.Attribute).IsType(CameraPropertyAttribute.TYPE_REAL))
                        return new CameraProperty<double>(camera, id, dataStruct, (x) => x);
                    else if (new CameraPropertyAttribute((uint)dataStruct.Attribute).IsType(CameraPropertyAttribute.TYPE_MODE))
                        return new CameraProperty<Mode>(camera, id, dataStruct, (x) => (Mode)x);
                    else
                        return new CameraProperty<int>(camera, id, dataStruct, (x) => (int)x);
            }
        }
    }
}
