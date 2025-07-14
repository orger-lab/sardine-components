using System.Runtime.InteropServices;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    [StructLayout(LayoutKind.Sequential,Pack=4)]
    public struct DCAMPropertyAttributeStruct
    {
        // input parameters
        public int Size;                    // size of this structure
        public int PropID;                     // DCAMIDPROPERTY
        public int Option;                    // DCAMPROPOTION
        public int ReservedID1;                // must be 0

        // output parameters
        public int Attribute;                 // DCAMPROPATTRIBUTE
        public int GroupID;                    // 0 reserved; DCAMIDGROUP
        public int UnitID;                     // DCAMPROPUNIT
        public int Attribute2;                // DCAMPROPATTRIBUTE2

        public double ValueMin;                 // minimum value
        public double ValueMax;                 // maximum value
        public double ValueStep;                // minimum stepping between a value and the next
        public double ValueDefault;             // default value

        // available from DCAM-API 3.0
        public int MaxChannel;               // max channel if supports
        public int ReservedID3;                // reserved to 0
        public int MaxView;                  // max view if supports

        // available from DCAM-API 3.1
        public int NumberOfElements;     // number of elements for array
        public int ArrayBase;           // base id of array if element
        public int ElementStep;         // step for iProp to next element

        public DCAMPropertyAttributeStruct(int _iprop)
        {
            Size = Marshal.SizeOf(typeof(DCAMPropertyAttributeStruct));
            PropID = _iprop;
            Option = 0;
            ReservedID1 = 0;
            Attribute = 0;
            GroupID = 0;
            UnitID = 0;
            Attribute2 = 0;
            ValueMin = 0;
            ValueMax = 0;
            ValueStep = 0;
            ValueDefault = 0;
            MaxChannel = 0;
            ReservedID3 = 0;
            MaxView = 0;
            NumberOfElements = 0;
            ArrayBase = 0;
            ElementStep = 0;
        }
    }
}