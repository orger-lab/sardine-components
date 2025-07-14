using System.Runtime.InteropServices;
using System.Text;

namespace Sardine.Utils.Windows
{
    public static class DisableHardware
    {
        const uint DIF_PROPERTYCHANGE = 0x12;
        const uint DICS_ENABLE = 1;
        const uint DICS_DISABLE = 2;
        const uint DICS_FLAG_GLOBAL = 1;
        const uint DIGCF_ALLCLASSES = 4;
        const uint ERROR_INVALID_DATA = 13;
        const uint ERROR_NO_MORE_ITEMS = 259;

        [StructLayout(LayoutKind.Sequential)]
        struct SP_CLASSINSTALL_HEADER
        {
            public UInt32 cbSize;
            public UInt32 InstallFunction;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct SP_PROPCHANGE_PARAMS
        {
            public SP_CLASSINSTALL_HEADER ClassInstallHeader;
            public UInt32 StateChange;
            public UInt32 Scope;
            public UInt32 HwProfile;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct SP_DEVINFO_DATA
        {
            public UInt32 cbSize;
            public Guid classGuid;
            public UInt32 devInst;
            public IntPtr reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct DEVPROPKEY
        {
            public Guid fmtid;
            public UInt32 pid;
        }

        [DllImport("setupapi.dll", SetLastError = true)]
        static extern IntPtr SetupDiGetClassDevsW(
            [In] ref Guid ClassGuid,
            [MarshalAs(UnmanagedType.LPWStr)]
            string? Enumerator,
            IntPtr parent,
            UInt32 flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        static extern bool SetupDiDestroyDeviceInfoList(IntPtr handle);

        [DllImport("setupapi.dll", SetLastError = true)]
        static extern bool SetupDiEnumDeviceInfo(IntPtr deviceInfoSet,
            UInt32 memberIndex,
            [Out] out SP_DEVINFO_DATA deviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true)]
        static extern bool SetupDiSetClassInstallParams(
            IntPtr deviceInfoSet,
            [In] ref SP_DEVINFO_DATA deviceInfoData,
            [In] ref SP_PROPCHANGE_PARAMS classInstallParams,
            UInt32 ClassInstallParamsSize);

        [DllImport("setupapi.dll", SetLastError = true)]
        static extern bool SetupDiChangeState(
            IntPtr deviceInfoSet,
            [In] ref SP_DEVINFO_DATA deviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true)]
        static extern bool SetupDiGetDevicePropertyW(
                IntPtr deviceInfoSet,
                [In] ref SP_DEVINFO_DATA DeviceInfoData,
                [In] ref DEVPROPKEY propertyKey,
                [Out] out UInt32 propertyType,
                IntPtr propertyBuffer,
                UInt32 propertyBufferSize,
                out UInt32 requiredSize,
                UInt32 flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        static extern bool SetupDiGetDeviceRegistryPropertyW(
          IntPtr DeviceInfoSet,
          [In] ref SP_DEVINFO_DATA DeviceInfoData,
          UInt32 Property,
          [Out] out UInt32 PropertyRegDataType,
          IntPtr PropertyBuffer,
          UInt32 PropertyBufferSize,
          [In, Out] ref UInt32 RequiredSize
        );


        public static List<List<string>> ListDevices()
        {
            IntPtr info = IntPtr.Zero;
            Guid NullGuid = Guid.Empty;
            List<List<string>> strings = new();
            try
            {
                info = SetupDiGetClassDevsW(
                    ref NullGuid,
                    null,
                    IntPtr.Zero,
                    DIGCF_ALLCLASSES);
                CheckError("SetupDiGetClassDevs");

                SP_DEVINFO_DATA devdata = new();
                devdata.cbSize = (uint)Marshal.SizeOf(devdata);

                for (uint i = 0; ; i++)
                {
                    SetupDiEnumDeviceInfo(info,
                        i,
                        out devdata);

                    CheckError("SetupDiEnumDeviceInfo");

                    string? devicepath = GetStringPropertyForDevice(info,
                                               devdata, 1); // SPDRP_HARDWAREID

                    if (devicepath is not null)
                    {
                        string? devicedata = GetStringPropertyForDevice(info, devdata, 0);
                        if (devicedata is not null)
                            strings.Add(new List<string> { devicepath,devicedata });
                    }
                }
            }
            catch { }
            finally
            {
                if (info != IntPtr.Zero)
                    _ = SetupDiDestroyDeviceInfoList(info);
            }

            return strings;
        }

        public static bool ResetDevice(Func<string, bool> filter)
        {
            if (ChangeDeviceStatus(filter, disable: true))
                if (ChangeDeviceStatus(filter, disable: false))
                    return true;

            return false;
        }

        public static bool ChangeDeviceStatus(Func<string, bool> filter, bool disable)
        {
            IntPtr info = IntPtr.Zero;
            Guid NullGuid = Guid.Empty;
            bool toReturn = true;
            try
            {
                info = SetupDiGetClassDevsW(
                    ref NullGuid,
                    null,
                    IntPtr.Zero,
                    DIGCF_ALLCLASSES);
                CheckError("SetupDiGetClassDevs");

                SP_DEVINFO_DATA devdata = new();
                devdata.cbSize = (uint)Marshal.SizeOf(devdata);

                for (uint i = 0; ; i++)
                {
                    SetupDiEnumDeviceInfo(info,
                        i,
                        out devdata);

                    if (Marshal.GetLastWin32Error() == ERROR_NO_MORE_ITEMS)
                        return true;
                    CheckError("SetupDiEnumDeviceInfo");

                    string? devicepath = GetStringPropertyForDevice(info,
                                               devdata, 1); // SPDRP_HARDWAREID


                    if (devicepath is not null && filter(devicepath)) break;

                }

                SP_CLASSINSTALL_HEADER header = new();
                header.cbSize = (uint)Marshal.SizeOf(header);
                header.InstallFunction = DIF_PROPERTYCHANGE;

                SP_PROPCHANGE_PARAMS propchangeparams = new()
                {
                    ClassInstallHeader = header,
                    StateChange = disable ? DICS_DISABLE : DICS_ENABLE,
                    Scope = DICS_FLAG_GLOBAL,
                    HwProfile = 0
                };

                SetupDiSetClassInstallParams(info,
                    ref devdata,
                    ref propchangeparams,
                    (uint)Marshal.SizeOf(propchangeparams));
                CheckError("SetupDiSetClassInstallParams");

                SetupDiChangeState(
                    info,
                    ref devdata);
            }
            catch { toReturn = false; }
            finally
            {
                if (info != IntPtr.Zero)
                    SetupDiDestroyDeviceInfoList(info);
            }
            return toReturn;
        }
        private static void CheckError(string message, int lasterror = -1)
        {

            int code = lasterror == -1 ? Marshal.GetLastWin32Error() : lasterror;
            if (code != 0)
                throw new ApplicationException(string.Format("Error disabling hardware device (Code {0}): {1}", code, message));
        }

        private static string? GetStringPropertyForDevice(IntPtr info, SP_DEVINFO_DATA devdata,
            uint propId)
        {
            uint outsize;
            IntPtr buffer = IntPtr.Zero;
            try
            {
                outsize = 0;
                SetupDiGetDeviceRegistryPropertyW(info, ref devdata, propId, out uint proptype, IntPtr.Zero, 0, ref outsize);

                uint buflen = outsize;
                buffer = Marshal.AllocHGlobal((int)buflen);
                outsize = 0;

                SetupDiGetDeviceRegistryPropertyW(
                    info,
                    ref devdata,
                    propId,
                    out proptype,
                    buffer,
                    buflen,
                    ref outsize);
                byte[] lbuffer = new byte[outsize];
                Marshal.Copy(buffer, lbuffer, 0, (int)outsize);
                int errcode = Marshal.GetLastWin32Error();
                if (errcode == ERROR_INVALID_DATA) return null;
                CheckError("SetupDiGetDeviceProperty", errcode);
                return Encoding.Unicode.GetString(lbuffer);
            }
            finally
            {
                if (buffer != IntPtr.Zero)
                    Marshal.FreeHGlobal(buffer);
            }
        }
    }
}