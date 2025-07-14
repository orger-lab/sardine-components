using System;

namespace Sardine.Devices.NI.DAQ.Controllers
{
    public partial class BufferedDAQChannelReader
    {
        public class BufferOverflowException : Exception
        {
            public BufferOverflowException(BufferedDAQChannelReader bufferedReader) : base($"Buffer overflow in {bufferedReader.analogTask?.MainTask.Name ?? bufferedReader.digitalTask?.MainTask.Name ?? "EMPTY_READER"}") { }
        }
    }
}
