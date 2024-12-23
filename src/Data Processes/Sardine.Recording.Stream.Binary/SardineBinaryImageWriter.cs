using Sardine.Core.DataModel;
using Sardine.ImageProcessing;

namespace Sardine.Recording.Stream.Binary
{

    /// <summary>
    /// Streams images to a file as binary data.
    /// </summary>
    public class SardineBinaryImageWriter : IDisposable
    {
        public static void Sink(SardineBinaryImageWriter handle, IImageFrame data, MessageMetadata metadata)
        {
            int senderID = metadata.Sender;

            if (!handle.headerWritten.Contains(senderID))
            {
                handle.fileStream.Add(senderID, new BinaryWriter(File.Open(System.IO.Path.ChangeExtension(System.IO.Path.Combine(handle.Path, $"{handle.FileName}_{metadata.SenderName.ToLowerInvariant()}"), ".sardis"), FileMode.Create)));
                handle.fileStream[senderID].Write(data.UnitXSize);
                handle.fileStream[senderID].Write(data.UnitYSize);
                handle.fileStream[senderID].Write(data.UnitZSize);
                handle.fileStream[senderID].Write(data.Width);
                handle.fileStream[senderID].Write(data.Height);
                handle.fileStream[senderID].Write(data.Planes);
                handle.fileStream[senderID].Write(data.CameraModel[..Math.Min(20, data.CameraModel.Length)].PadLeft(20));
                handle.fileStream[senderID].Write(data.CameraSN[..Math.Min(20, data.CameraSN.Length)].PadLeft(20));
                handle.headerWritten.Add(senderID);
            }

            if (data.IsAlive)
            {
                handle.fileStream[senderID].Write(data.FrameID);
                for (int i = 0; i < data.Planes; i++)
                {
                    handle.fileStream[senderID].Write(data.Timestamps[i].Ticks);
                    handle.fileStream[senderID].Write(data.FrameData![i]);
                }
            }
        }

        public string Path
        {
            get; set;
        } = "";
        public string FileName { get; set; } = "";


        private HashSet<int> headerWritten;

        private Dictionary<int, BinaryWriter> fileStream;
        private bool disposedValue;

        public SardineBinaryImageWriter()
        {
            headerWritten = new HashSet<int>();
            fileStream = new Dictionary<int, BinaryWriter>();
            // senderName = new();
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
    }
}
