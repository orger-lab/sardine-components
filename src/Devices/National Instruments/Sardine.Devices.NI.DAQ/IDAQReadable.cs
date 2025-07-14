using System.Collections.Generic;

namespace Sardine.Devices.NI.DAQ
{
    public interface IDAQReadable
    {
        public IReadOnlyList<DaqPhysicalChannelID> GetChannels();
    }
}
