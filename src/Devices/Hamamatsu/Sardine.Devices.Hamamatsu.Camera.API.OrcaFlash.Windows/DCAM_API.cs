using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Sardine.Devices.Hamamatsu.Camera.API.OrcaFlash.Windows
{
    // ##########################
    // ####  API Access - Native
    // ##########################
    /// <summary>
    /// Static class to access the DCAM-API library
    /// </summary>
    static class DCAM_API
    {
        /// <summary>
        /// DCAM_API library file
        /// </summary>
        const string LibName = "lib\\dcamapi.dll";

        public static string? GetNativeVersion() => FileVersionInfo.GetVersionInfo(LibName).FileVersion;

        #region Initialization
        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamapi_init(ref DCAMAPIInit param);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamapi_uninit();

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamdev_open(ref DCAMDevOpen param);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamdev_close(IntPtr h);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamdev_getstring(IntPtr h, ref DCAMDevString param);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamdev_showpanel(IntPtr h, int iKind);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamdev_getcapability(IntPtr h, ref DCAMDevInit param);
        #endregion


        #region Buffer Control
        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcambuf_alloc(IntPtr h, int framecount);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcambuf_release(IntPtr h, int iKind);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcambuf_lockframe(IntPtr h, ref DCAMBufferFrame pFrame);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcambuf_attach(IntPtr h, ref DCAMBufferAttach param);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcambuf_copyframe(IntPtr h, ref DCAMBufferFrame pFrame);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcambuf_copymetadata(IntPtr h, ref  DCAMMetadataHDR hdr);
        #endregion


        #region Capture
        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamcap_start(IntPtr h, int mode);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamcap_capture(IntPtr h);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamcap_stop(IntPtr h);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamcap_status(IntPtr h, ref CaptureStatus pStatus);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamcap_transferinfo(IntPtr h, ref DCAMCaptureTransferInfo param);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamcap_firetrigger(IntPtr h, int iKind);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamcap_record(IntPtr h, IntPtr hdcamrec);
        #endregion


        #region Wait Handle Control
        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamwait_open(ref DCAMWaitOpen param);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamwait_close(IntPtr hWait);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamwait_start(IntPtr hWait, ref DCAMWaitStart param);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamwait_abort(IntPtr hWait);
        #endregion


        #region Recording
        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamrec_openA(ref DCAMRecordOpen param);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamrec_status(IntPtr hdcamrec, ref DCAMRecordStatus param);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamrec_close(IntPtr hdcamrec);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamrec_lockframe(IntPtr hdcamrec, ref DCAMRecordFrame pFrame);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamrec_copyframe(IntPtr hdcamrec, ref DCAMRecordFrame pFrame);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamrec_writemetadata(IntPtr hdcamrec, ref DCAMMetadataHDR hdr);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamrec_lockmetadata(IntPtr hdcamrec, ref DCAMMetadataHDR hdr);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamrec_copymetadata(IntPtr hdcamrec, ref DCAMMetadataHDR hdr);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamrec_lockmetadatablock(IntPtr hdcamrec, ref DCAMMetadataBlockHDR hdr);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamrec_copymetadatablock(IntPtr hdcamrec, ref DCAMMetadataBlockHDR hdr);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamrec_pause(IntPtr hdcamrec);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamrec_resume(IntPtr hdcamrec);
        #endregion


        #region Property control
        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamprop_getattr(IntPtr h, ref DCAMPropertyAttributeStruct param);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamprop_getvalue(IntPtr h, int iProp, ref double pValue);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamprop_setvalue(IntPtr h, int iProp, double fValue);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamprop_setgetvalue(IntPtr h, int iProp, ref double pValue, int option);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamprop_queryvalue(IntPtr h, int iProp, ref double pValue, int option);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamprop_getnextid(IntPtr h, ref int iProp, int option);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)] //Marshal as LPWStr ?
        public static extern HamamatsuError dcamprop_getname(IntPtr h, int iProp, [MarshalAs(UnmanagedType.LPStr)] StringBuilder text, int textbytes);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcamprop_getvaluetext(IntPtr h, ref DCAMPropertyValueText param);
        #endregion


        #region Weirdos
        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcam_getbinning(IntPtr h, out int pBinning);

        [DllImport(LibName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern HamamatsuError dcam_setbinning(IntPtr h, int binning);
        #endregion
    }
}