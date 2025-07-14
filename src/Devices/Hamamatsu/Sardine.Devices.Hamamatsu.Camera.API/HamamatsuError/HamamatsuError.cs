using System;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    public partial struct HamamatsuError : IEquatable<HamamatsuError>
    {
        private readonly ERR _value;

        public static readonly HamamatsuError Busy = new(ERR.BUSY);
        public static readonly HamamatsuError NotReady = new(ERR.NOTREADY);
        public static readonly HamamatsuError NotStable = new(ERR.NOTSTABLE);
        public static readonly HamamatsuError Unstable = new(ERR.UNSTABLE);
        public static readonly HamamatsuError NotBusy = new(ERR.NOTBUSY);
        public static readonly HamamatsuError Excluded = new(ERR.EXCLUDED);
        public static readonly HamamatsuError CoolingTrouble = new(ERR.COOLINGTROUBLE);
        public static readonly HamamatsuError NoTrigger = new(ERR.NOTRIGGER);
        public static readonly HamamatsuError TemperatureTrouble = new(ERR.TEMPERATURE_TROUBLE);
        public static readonly HamamatsuError TooFrequentTrigger = new(ERR.TOOFREQUENTTRIGGER);
        public static readonly HamamatsuError Abort = new(ERR.ABORT);
        public static readonly HamamatsuError Timeout = new(ERR.TIMEOUT);
        public static readonly HamamatsuError LostFrame = new(ERR.LOSTFRAME);
        public static readonly HamamatsuError MissingFrameTrouble = new(ERR.MISSINGFRAME_TROUBLE);
        public static readonly HamamatsuError InvalidImage = new(ERR.INVALIDIMAGE);
        public static readonly HamamatsuError NoResource = new(ERR.NORESOURCE);
        public static readonly HamamatsuError NoMemory = new(ERR.NOMEMORY);
        public static readonly HamamatsuError NoModule = new(ERR.NOMODULE);
        public static readonly HamamatsuError NoDriver = new(ERR.NODRIVER);
        public static readonly HamamatsuError NoCamera = new(ERR.NOCAMERA);
        public static readonly HamamatsuError NoGrabber = new(ERR.NOGRABBER);
        public static readonly HamamatsuError NoCombination = new(ERR.NOCOMBINATION);
        public static readonly HamamatsuError FailOpen = new(ERR.FAILOPEN);
        public static readonly HamamatsuError InvalideModule = new(ERR.INVALIDMODULE);
        public static readonly HamamatsuError InvalidCOMPort = new(ERR.INVALIDCOMMPORT);
        public static readonly HamamatsuError FailOpenBus = new(ERR.FAILOPENBUS);
        public static readonly HamamatsuError FailOpenCamera = new(ERR.FAILOPENCAMERA);
        public static readonly HamamatsuError FramegrabberNeedsFirmwareUpdate = new(ERR.FRAMEGRABBER_NEEDS_FIRMWAREUPDATE);
        public static readonly HamamatsuError InvalidCamera = new(ERR.INVALIDCAMERA);
        public static readonly HamamatsuError InvalidHandle = new(ERR.INVALIDHANDLE);
        public static readonly HamamatsuError InvalidParam = new(ERR.INVALIDPARAM);
        public static readonly HamamatsuError InvalidValue = new(ERR.INVALIDVALUE);
        public static readonly HamamatsuError OutOfRange = new(ERR.OUTOFRANGE);
        public static readonly HamamatsuError NotWriteable = new(ERR.NOTWRITABLE);
        public static readonly HamamatsuError NotReadable = new(ERR.NOTREADABLE);
        public static readonly HamamatsuError InvalidPropertyID = new(ERR.INVALIDPROPERTYID);
        public static readonly HamamatsuError NewAPIRequired = new(ERR.NEWAPIREQUIRED);
        public static readonly HamamatsuError WrongHandshake = new(ERR.WRONGHANDSHAKE);
        public static readonly HamamatsuError NoProperty = new(ERR.NOPROPERTY);
        public static readonly HamamatsuError InvalidChannel = new(ERR.INVALIDCHANNEL);
        public static readonly HamamatsuError InvalidView = new(ERR.INVALIDVIEW);
        public static readonly HamamatsuError InvalidSubarray = new(ERR.INVALIDSUBARRAY);
        public static readonly HamamatsuError AccessDeny = new(ERR.ACCESSDENY);
        public static readonly HamamatsuError NoValueText = new(ERR.NOVALUETEXT);
        public static readonly HamamatsuError WrongPropertyValue = new(ERR.WRONGPROPERTYVALUE);
        public static readonly HamamatsuError Disharmony = new(ERR.DISHARMONY);
        public static readonly HamamatsuError FramebundleShouldBeOff = new(ERR.FRAMEBUNDLESHOULDBEOFF);
        public static readonly HamamatsuError InvalidFrameIndex = new(ERR.INVALIDFRAMEINDEX);
        public static readonly HamamatsuError InvalidSessionIndex = new(ERR.INVALIDSESSIONINDEX);
        public static readonly HamamatsuError NoCorrectionData = new(ERR.NOCORRECTIONDATA);
        public static readonly HamamatsuError ChannelDepdendentValue = new(ERR.CHANNELDEPENDENTVALUE);
        public static readonly HamamatsuError ViewDependentValue = new(ERR.VIEWDEPENDENTVALUE);
        public static readonly HamamatsuError InvalidCalibSetting = new(ERR.INVALIDCALIBSETTING);
        public static readonly HamamatsuError LessSystemMemory = new(ERR.LESSSYSTEMMEMORY);
        public static readonly HamamatsuError NotSupport = new(ERR.NOTSUPPORT);
        public static readonly HamamatsuError FailReadCamera = new(ERR.FAILREADCAMERA);
        public static readonly HamamatsuError FailWriteCamera = new(ERR.FAILWRITECAMERA);
        public static readonly HamamatsuError ConflictCOMPort = new(ERR.CONFLICTCOMMPORT);
        public static readonly HamamatsuError OpticsUnplugged = new(ERR.OPTICS_UNPLUGGED);
        public static readonly HamamatsuError FailCalibration = new(ERR.FAILCALIBRATION);
        public static readonly HamamatsuError InvalidMember3 = new(ERR.INVALIDMEMBER_3);
        public static readonly HamamatsuError InvalidMember5 = new(ERR.INVALIDMEMBER_5);
        public static readonly HamamatsuError InvalidMember7 = new(ERR.INVALIDMEMBER_7);
        public static readonly HamamatsuError InvalidMember8 = new(ERR.INVALIDMEMBER_8);
        public static readonly HamamatsuError InvalidMember9 = new(ERR.INVALIDMEMBER_9);
        public static readonly HamamatsuError FailedOpenRecFile = new(ERR.FAILEDOPENRECFILE);
        public static readonly HamamatsuError InvalidRecHandle = new(ERR.INVALIDRECHANDLE);
        public static readonly HamamatsuError FailedWriteData = new(ERR.FAILEDWRITEDATA);
        public static readonly HamamatsuError FailedReadData = new(ERR.FAILEDREADDATA);
        public static readonly HamamatsuError NowRecording = new(ERR.NOWRECORDING);
        public static readonly HamamatsuError WriteFull = new(ERR.WRITEFULL);
        public static readonly HamamatsuError AlreadyOccupied = new(ERR.ALREADYOCCUPIED);
        public static readonly HamamatsuError TooLArgeUserDataSize = new(ERR.TOOLARGEUSERDATASIZE);
        public static readonly HamamatsuError NoImage = new(ERR.NOIMAGE);
        public static readonly HamamatsuError InvalidWaitHandle = new(ERR.INVALIDWAITHANDLE);
        public static readonly HamamatsuError NewRuntimeRequired = new(ERR.NEWRUNTIMEREQUIRED);
        public static readonly HamamatsuError VersionMismatch = new(ERR.VERSIONMISMATCH);
        public static readonly HamamatsuError RunasFactoryMode = new(ERR.RUNAS_FACTORYMODE);
        public static readonly HamamatsuError ImageUnknownSignature = new(ERR.IMAGE_UNKNOWNSIGNATURE);
        public static readonly HamamatsuError ImageNewRuntimeRequired = new(ERR.IMAGE_NEWRUNTIMEREQUIRED);
        public static readonly HamamatsuError ImageErrorStatusExist = new(ERR.IMAGE_ERRORSTATUSEXIST);
        public static readonly HamamatsuError ImageHeaderCorrupted = new(ERR.IMAGE_HEADERCORRUPTED);
        public static readonly HamamatsuError ImageBrokenContext = new(ERR.IMAGE_BROKENCONTENT);
        public static readonly HamamatsuError UnknownMessageID = new(ERR.UNKNOWNMSGID);
        public static readonly HamamatsuError UnknownStringID = new(ERR.UNKNOWNSTRID);
        public static readonly HamamatsuError UnknownParameterID = new(ERR.UNKNOWNPARAMID);
        public static readonly HamamatsuError UnknownBitsType = new(ERR.UNKNOWNBITSTYPE);
        public static readonly HamamatsuError UnknownDataType = new(ERR.UNKNOWNDATATYPE);
        public static readonly HamamatsuError None = new(ERR.NONE);
        public static readonly HamamatsuError InstallationInProgress = new(ERR.INSTALLATIONINPROGRESS);
        public static readonly HamamatsuError Unreach = new(ERR.UNREACH);
        public static readonly HamamatsuError Unloaded = new(ERR.UNLOADED);
        public static readonly HamamatsuError ThroughAdapter = new(ERR.THRUADAPTER);
        public static readonly HamamatsuError NoConnection = new(ERR.NOCONNECTION);
        public static readonly HamamatsuError NotImplemented = new(ERR.NOTIMPLEMENT);
        public static readonly HamamatsuError APIInitInitOptionBytes = new(ERR.APIINIT_INITOPTIONBYTES);
        public static readonly HamamatsuError APIInitInitOption = new(ERR.APIINIT_INITOPTION);
        public static readonly HamamatsuError InitOptionCollisionBase = new(ERR.INITOPTION_COLLISION_BASE);
        public static readonly HamamatsuError InitOptionCollisionMax = new(ERR.INITOPTION_COLLISION_MAX);
        public static readonly HamamatsuError Success = new(ERR.SUCCESS);

        internal HamamatsuError(uint argv)
        {
            _value = (ERR)argv;
        }
        private HamamatsuError(ERR argv)
        {
            _value = argv;
        }

        public static implicit operator int(HamamatsuError self) => (int)self._value;
        public static bool operator ==(HamamatsuError a, HamamatsuError b) => a._value == b._value;
        public static bool operator !=(HamamatsuError a, HamamatsuError b) => a._value != b._value;

        public bool Equals(HamamatsuError err) => this == err;
        public override int GetHashCode() => _value.GetHashCode();
        public bool Failed() => (uint)_value >= 0x80000000;
        public override bool Equals(object? obj)
        {
            return obj is HamamatsuError error && Equals(error);
        }
    }
}