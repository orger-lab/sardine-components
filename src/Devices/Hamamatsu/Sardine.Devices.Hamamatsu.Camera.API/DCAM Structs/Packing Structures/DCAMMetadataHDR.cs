using System.Runtime.InteropServices;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    [StructLayout(LayoutKind.Sequential,Pack=4)]
    public struct DCAMMetadataHDR
    {
        public int Size;                      // [in] size of whole structure, not only this.
        public int KindID;                     // [in] DCAM_METADATAKIND
        public int Option;                    // [in] value meaning depends on DCAM_METADATAKIND
        public int FrameID;                    // [in] frame index

        public DCAMMetadataHDR(int metadatakind)
        {
            Size = Marshal.SizeOf(typeof(DCAMMetadataHDR));
            KindID = metadatakind;
            Option = 0;
            FrameID = 0;
            FrameID = 0;
        }
    }
}