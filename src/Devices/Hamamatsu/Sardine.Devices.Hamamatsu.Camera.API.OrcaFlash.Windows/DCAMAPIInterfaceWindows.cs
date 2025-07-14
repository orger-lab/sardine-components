using System;
using System.Text;
using System.Runtime.InteropServices;
using Sardine.Utils.Windows;

namespace Sardine.Devices.Hamamatsu.Camera.API.OrcaFlash.Windows
{
    public class DCAMAPIInterfaceWindows : IHamamatsuAPIWrapper
    {
        public string Version => @"DCAM_NativeToManaged_v1.0";
        public string NativeVersion => DCAM_API.GetNativeVersion() ?? "0";

        public HamamatsuError Init(ref DCAMAPIInit param) => DCAM_API.dcamapi_init(ref param);
        public HamamatsuError UnInit() => DCAM_API.dcamapi_uninit();
        public HamamatsuError OpenDev(ref DCAMDevOpen param) => DCAM_API.dcamdev_open(ref param);
        public HamamatsuError CloseDev(IntPtr hdcam) => DCAM_API.dcamdev_close(hdcam);
        public HamamatsuError GetCapabilityDev(IntPtr hdcam, ref DCAMDevInit param) => DCAM_API.dcamdev_getcapability(hdcam, ref param);
        public HamamatsuError AttachBuf(IntPtr h, ref DCAMBufferAttach param) => DCAM_API.dcambuf_attach(h, ref param);
        public HamamatsuError AllocBuf(IntPtr hdcam, int framecount) => DCAM_API.dcambuf_alloc(hdcam, framecount);
        public HamamatsuError ReleaseBuf(IntPtr hdcam, int iKind) => DCAM_API.dcambuf_release(hdcam, iKind);
        public HamamatsuError LockFrameBuf(IntPtr hdcam, ref DCAMBufferFrame aFrame) => DCAM_API.dcambuf_lockframe(hdcam, ref aFrame);
        public HamamatsuError CopyFrameBuf(IntPtr hdcam, ref DCAMBufferFrame aFrame) => DCAM_API.dcambuf_copyframe(hdcam, ref aFrame);
        public HamamatsuError CopyMetadataBuf(IntPtr hdcam, ref DCAMMetadataHDR hdr) => DCAM_API.dcambuf_copymetadata(hdcam, ref hdr);
        public HamamatsuError StartCap(IntPtr hdcam, CaptureSetting mode) => DCAM_API.dcamcap_start(hdcam, (int)mode);
        public HamamatsuError StopCap(IntPtr hdcam) => DCAM_API.dcamcap_stop(hdcam);
        public HamamatsuError StatusCap(IntPtr hdcam, ref CaptureStatus param) => DCAM_API.dcamcap_status(hdcam, ref param);
        public HamamatsuError TransferInfoCap(IntPtr hdcam, ref DCAMCaptureTransferInfo param) => DCAM_API.dcamcap_transferinfo(hdcam, ref param);
        public HamamatsuError FireTriggerCap(IntPtr hdcam, int iKind) => DCAM_API.dcamcap_firetrigger(hdcam, iKind);
        public HamamatsuError RecordCap(IntPtr hdcam, IntPtr hdcamrec) => DCAM_API.dcamcap_record(hdcam, hdcamrec);
        public HamamatsuError OpenWait(ref DCAMWaitOpen param) => DCAM_API.dcamwait_open(ref param);
        public HamamatsuError CloseWait(IntPtr hwait) => DCAM_API.dcamwait_close(hwait);
        public HamamatsuError StartWait(IntPtr hwait, ref DCAMWaitStart param) => DCAM_API.dcamwait_start(hwait, ref param);
        public HamamatsuError AbortWait(IntPtr hwait) => DCAM_API.dcamwait_abort(hwait);
        public HamamatsuError GetAttribute(IntPtr hdcam, ref DCAMPropertyAttributeStruct param) => DCAM_API.dcamprop_getattr(hdcam, ref param);
        public HamamatsuError GetValue(IntPtr hdcam, int iProp, ref double value)  => DCAM_API.dcamprop_getvalue(hdcam, iProp, ref value);
        public HamamatsuError SetValue(IntPtr hdcam, int iProp, double value) => DCAM_API.dcamprop_setvalue(hdcam, iProp, value);
        public HamamatsuError SetGetValue(IntPtr hdcam, int iProp, ref double value, DCAMPropertyOption _option) => DCAM_API.dcamprop_setgetvalue(hdcam, iProp, ref value, _option);
        public HamamatsuError QueryValue(IntPtr hdcam, int iProp, ref double value, DCAMPropertyOption _option) => DCAM_API.dcamprop_queryvalue(hdcam, iProp, ref value, _option);
        public HamamatsuError GetNextID(IntPtr hdcam, ref int iProp, int _option) => DCAM_API.dcamprop_getnextid(hdcam, ref iProp, _option);
        public HamamatsuError OpenRec(ref DCAMRecordOpen param) => DCAM_API.dcamrec_openA(ref param);
        public HamamatsuError StatusRec(IntPtr hdcamrec, ref DCAMRecordStatus param) => DCAM_API.dcamrec_status(hdcamrec, ref param);
        public HamamatsuError CloseRec(IntPtr hdcamrec) => DCAM_API.dcamrec_close(hdcamrec);
        public HamamatsuError LockFrameRec(IntPtr hdcamrec, ref DCAMRecordFrame aFrame) => DCAM_API.dcamrec_lockframe(hdcamrec, ref aFrame);
        public HamamatsuError CopyFrameRec(IntPtr hdcamrec, ref DCAMRecordFrame aFrame) => DCAM_API.dcamrec_copyframe(hdcamrec, ref aFrame);
        public HamamatsuError WriteMetadataRec(IntPtr hdcamrec, ref DCAMMetadataHDR hdr) => DCAM_API.dcamrec_writemetadata(hdcamrec, ref hdr);
        public HamamatsuError LockMetadataRec(IntPtr hdcamrec, ref DCAMMetadataHDR hdr) => DCAM_API.dcamrec_lockmetadata(hdcamrec, ref hdr);
        public HamamatsuError CopyMetadataRec(IntPtr hdcamrec, ref DCAMMetadataHDR hdr) => DCAM_API.dcamrec_copymetadata(hdcamrec, ref hdr);
        public HamamatsuError LockMetadataBlockRec(IntPtr hdcamrec, ref DCAMMetadataBlockHDR hdr) => DCAM_API.dcamrec_lockmetadatablock(hdcamrec, ref hdr);
        public HamamatsuError CopyMetadataBlockRec(IntPtr hdcamrec, ref DCAMMetadataBlockHDR hdr) => DCAM_API.dcamrec_copymetadatablock(hdcamrec, ref hdr);
        public HamamatsuError GetBinning(IntPtr hdcamrec, ref int value) => DCAM_API.dcam_getbinning(hdcamrec, out value);
        public HamamatsuError SetBinning(IntPtr hdcamrec, int value) => DCAM_API.dcam_setbinning(hdcamrec, value);
        public IHamamatsuAPIWrapper GetNew() => new DCAMAPIInterfaceWindows();

        public HamamatsuError GetStringDev(IntPtr hdcam, DCAMStringID idstr, ref string str)
        {
            DCAMDevString param = new(idstr)
            {
                TextBytes = 256
            };
            byte[] buf = new byte[param.TextBytes];
            GCHandle handle = GCHandle.Alloc(buf, GCHandleType.Pinned);
            param.Text = handle.AddrOfPinnedObject();

            HamamatsuError err;
            err = DCAM_API.dcamdev_getstring(hdcam, ref param);
            handle.Free();

            if (err.Failed())
            {
                str = "";
            }
            else
            {
                int i;
                for (i = 0; i < buf.Length; i++)
                {
                    if (buf[i] == 0)
                        break;
                }
                str = Encoding.ASCII.GetString(buf)[..i];
            }

            return err;
        }
        public HamamatsuError GetName(IntPtr hdcam, int iProp, ref string ret)
        {
            int textbytes = 256;
            StringBuilder sb = new(textbytes);

            HamamatsuError err;
            err = DCAM_API.dcamprop_getname(hdcam, iProp, sb, textbytes);
            if (!err.Failed())
                ret = sb.ToString();

            return err;
        }
        public HamamatsuError GetValueText(IntPtr hdcam, int iProp, double value, ref string ret)
        {
            DCAMPropertyValueText param = new(iProp, value)
            {
                TextBytes = 256
            };
            byte[] buf = new byte[param.TextBytes];
            GCHandle handle = GCHandle.Alloc(buf, GCHandleType.Pinned);
            param.Text = handle.AddrOfPinnedObject();

            HamamatsuError err;
            err = DCAM_API.dcamprop_getvaluetext(hdcam, ref param);
            handle.Free();

            if (err.Failed())
            {
                ret = "";
            }
            else
            {
                int i;
                for (i = 0; i < buf.Length; i++)
                {
                    if (buf[i] == 0)
                        break;
                }
                ret = Encoding.ASCII.GetString(buf)[..i];
            }

            return err;
        }
        public double GetAvailableMemory()
        {
            return MemoryStatusFetcher.GetMemoryStatus().Free;
        }

        public void PreInitialize()
        {
            ProcessHelper.KillProcess("DCAMTRAY");
        }

    }
}