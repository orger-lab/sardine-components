using Sardine.Core.DataModel;

namespace Sardine.Recording.Data.Text
{

    /// <summary>
    /// Component that saves text messages as text files.
    /// </summary>
    public class TextMessageRecorder : IDisposable
    {
        public string Path
        {
            get; set;
        } = "";
        public string FileName { get; set; } = "";

        private HashSet<int> headerNotWritten;

        private Dictionary<int, StreamWriter> fileStream;
        private bool disposedValue;

        public TextMessageRecorder()
        {
            headerNotWritten = new HashSet<int>();
            fileStream = new Dictionary<int, StreamWriter>();
        }

        /// <summary>
        /// Closes all the streams used to save data.
        /// </summary>
        public void CloseStreams()
        {
            foreach (var item in fileStream.Values)
            {
                item.Flush();
                item.Close();
            }
            fileStream.Clear();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    CloseStreams();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public static void Sink(TextMessageRecorder handle, ITextWritable data, MessageMetadata metadata)
        {
            int senderID = metadata.Sender;

            if (!handle.headerNotWritten.Contains(senderID))
            {
                handle.fileStream.Add(senderID, new StreamWriter(System.IO.Path.ChangeExtension(System.IO.Path.Combine(handle.Path, $"{handle.FileName}_{metadata.SenderName.ToLowerInvariant()}"), ".txt")));
                handle.fileStream[senderID].WriteLine(data.WriteHeader());
                handle.headerNotWritten.Add(senderID);
            }

            if (data.IsAlive)
                handle.fileStream[senderID].WriteLine(data.WriteData());

        }
    }
}