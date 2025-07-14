namespace Sardine.Devices.NI.DAQ
{
    public class DaqInternalChannelID
    {
        public InternalChannelAccess ChannelType { get; }
        public string ChannelName { get; }
   
        public DaqInternalChannelID(string name, InternalChannelAccess type)
        {
            ChannelType = type;
            ChannelName = name;
        }
        public override string ToString() => ChannelName;
    }
}
