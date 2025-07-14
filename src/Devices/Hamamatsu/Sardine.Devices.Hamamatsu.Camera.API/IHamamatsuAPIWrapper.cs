using System;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    // ##########################
    // ####  API Access - C#
    // ##########################
    public interface IHamamatsuAPIWrapper
    {
        public IHamamatsuAPIWrapper GetNew();
        public string Version { get; }
        public string NativeVersion { get; }
        public HamamatsuError Init(ref DCAMAPIInit param);
        public HamamatsuError UnInit();

        public HamamatsuError OpenDev(ref DCAMDevOpen param);
        public HamamatsuError CloseDev(IntPtr hdcam);
        public HamamatsuError GetStringDev(IntPtr hdcam, DCAMStringID idstr, ref string str);
        public HamamatsuError GetCapabilityDev(IntPtr hdcam, ref DCAMDevInit param);

        public HamamatsuError AttachBuf(IntPtr h, ref DCAMBufferAttach param);
        public HamamatsuError AllocBuf(IntPtr hdcam, int framecount);
        public HamamatsuError ReleaseBuf(IntPtr hdcam, int iKind);
        public HamamatsuError LockFrameBuf(IntPtr hdcam, ref DCAMBufferFrame aFrame);
        public HamamatsuError CopyFrameBuf(IntPtr hdcam, ref DCAMBufferFrame aFrame);
        public HamamatsuError CopyMetadataBuf(IntPtr hdcam, ref DCAMMetadataHDR hdr);

        public HamamatsuError StartCap(IntPtr hdcam, CaptureSetting mode);
        public HamamatsuError StopCap(IntPtr hdcam);
        public HamamatsuError StatusCap(IntPtr hdcam, ref CaptureStatus param);
        public HamamatsuError TransferInfoCap(IntPtr hdcam, ref DCAMCaptureTransferInfo param);
        public HamamatsuError FireTriggerCap(IntPtr hdcam, int iKind);
        public HamamatsuError RecordCap(IntPtr hdcam, IntPtr hdcamrec);

        public HamamatsuError OpenWait(ref DCAMWaitOpen param);
        public HamamatsuError CloseWait(IntPtr hwait);
        public HamamatsuError StartWait(IntPtr hwait, ref DCAMWaitStart param);
        public HamamatsuError AbortWait(IntPtr hwait);

        public HamamatsuError GetAttribute(IntPtr hdcam, ref DCAMPropertyAttributeStruct param);
        public HamamatsuError GetValue(IntPtr hdcam, int iProp, ref double value);
        public HamamatsuError SetValue(IntPtr hdcam, int iProp, double value);
        public HamamatsuError SetGetValue(IntPtr hdcam, int iProp, ref double value, DCAMPropertyOption _option);
        public HamamatsuError QueryValue(IntPtr hdcam, int iProp, ref double value, DCAMPropertyOption _option);
        public HamamatsuError GetNextID(IntPtr hdcam, ref int iProp, int _option);
        public HamamatsuError GetName(IntPtr hdcam, int iProp, ref string ret);
        public HamamatsuError GetValueText(IntPtr hdcam, int iProp, double value, ref string ret);

        public HamamatsuError OpenRec(ref DCAMRecordOpen param);
        public HamamatsuError StatusRec(IntPtr hdcamrec, ref DCAMRecordStatus param);
        public HamamatsuError CloseRec(IntPtr hdcamrec);
        public HamamatsuError LockFrameRec(IntPtr hdcamrec, ref DCAMRecordFrame aFrame);
        public HamamatsuError CopyFrameRec(IntPtr hdcamrec, ref DCAMRecordFrame aFrame);
        public HamamatsuError WriteMetadataRec(IntPtr hdcamrec, ref DCAMMetadataHDR hdr);
        public HamamatsuError LockMetadataRec(IntPtr hdcamrec, ref DCAMMetadataHDR hdr);
        public HamamatsuError CopyMetadataRec(IntPtr hdcamrec, ref DCAMMetadataHDR hdr);
        public HamamatsuError LockMetadataBlockRec(IntPtr hdcamrec, ref DCAMMetadataBlockHDR hdr);
        public HamamatsuError CopyMetadataBlockRec(IntPtr hdcamrec, ref DCAMMetadataBlockHDR hdr);
        public HamamatsuError GetBinning(IntPtr hdcamrec, ref int value);
        public HamamatsuError SetBinning(IntPtr hdcamrec, int value);

        public double GetAvailableMemory();

        public void PreInitialize();
    }
}