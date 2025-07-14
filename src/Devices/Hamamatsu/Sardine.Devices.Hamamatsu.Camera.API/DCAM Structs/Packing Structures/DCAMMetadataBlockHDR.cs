using System.Runtime.InteropServices;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    [StructLayout(LayoutKind.Sequential,Pack=4)]
    public struct DCAMMetadataBlockHDR
    {
        public int Size;                      // [in] size of whole structure, not only this.
        public int KindID;                     // [in] DCAM_METADATAKIND
        public int Option;                    // [in] value meaning depends on DCAMBUF_METADATAOPTION or DCAMREC_METADATAOPTION
        public int FrameID;                    // [in] start frame index
        public int InCount;                  // [in] max count of meta data
        public int OutCount;                  // [out] count of got meta data.

        public DCAMMetadataBlockHDR(int metadatakind)
        {
            Size = Marshal.SizeOf(typeof(DCAMMetadataBlockHDR));
            KindID = metadatakind;
            Option = 0;
            FrameID = 0;
            InCount = 0;
            OutCount = 0;
        }
    }
}