using Sardine.ImageProcessing;

namespace Sardine.Test.BinaryStreamToImageFrameReader
{
    public partial class BinaryStreamReader : IDisposable
    {
        private bool disposedValue;
        BinaryReader? Reader { get; set; }
        public ImageFrameInfo? Header { get; private set; }

        readonly object readerAccessLock = new();

        public BinaryStreamReader(string path, int skipFirst = 0)
        {
            Path = path;
            SkipFirst = skipFirst;
            OpenStream();
        }

        public string Path { get; }
        public int SkipFirst { get; }

        void OpenStream()
        {
            lock (readerAccessLock)
            {
                if (Reader is not null)
                    CloseStream();

                Reader = new BinaryReader(File.Open(Path, FileMode.Open));



                double unitXSize = Reader.ReadDouble();
                double unitYSize = Reader.ReadDouble();
                double unitZSize = Reader.ReadDouble();
                int width = Reader.ReadInt32();
                int height = Reader.ReadInt32();
                int planes = Reader.ReadInt32();
                string cameraModel = Reader.ReadString();
                string cameraSN = Reader.ReadString();

                int frameSize = 4 + planes * (8 + width * height);
                int headerSize = (int)Reader.BaseStream.Position;

                Header = new ImageFrameInfo(unitXSize, unitYSize, unitZSize, width, height, planes, cameraModel, cameraSN, frameSize, headerSize);


                Reader.BaseStream.Seek(frameSize * SkipFirst, SeekOrigin.Current);
            }
        }

        public ImageFrame? ReadFrameAt(int index)
        {
            if (Reader is null)
                return null;

            if (Header is null)
                return null;

            lock (readerAccessLock)
            {
                try
                {
                    Reader.BaseStream.Seek(Header.HeaderSize + (long)(index) * Header.FrameSize, SeekOrigin.Begin);
                }
                catch (IOException) { return null; }


                ImageFrame? frame = null;

                try
                {
                    frame = ReadNextFrame();
                }
                catch (EndOfStreamException) { }

                return frame;
            }
        }

        public (int FrameID, IReadOnlyList<DateTime> Timestamps)? ReadMetadataAt(int index)
        {
            if (Reader is null)
                return null;

            if (Header is null)
                return null;


            (int FrameID, IReadOnlyList<DateTime> Timestamps)? metadata = null;

            lock (readerAccessLock)
            {
                try
                {
                    Reader.BaseStream.Seek(Header.HeaderSize + (long)(index) * Header.FrameSize, SeekOrigin.Begin);
                }
                catch (IOException) { return null; }

                try
                {
                    int frameID = Reader.ReadInt32();

                    DateTime[] timestamps = new DateTime[Header.Planes];

                    for (int i = 0; i < Header.Planes; i++)
                    {
                        timestamps[i] = new DateTime(Reader.ReadInt64());
                        Reader.BaseStream.Seek(Header.Width * Header.Height, SeekOrigin.Current);
                    }

                    metadata = (frameID, timestamps);
                }
                catch (EndOfStreamException) { return null; }
            }
            
            return metadata;
        }


        public ImageFrame? ReadNextFrame()
        {
            if (Reader is null)
                return null;

            if (Header is null)
                return null;

            lock (readerAccessLock)
            {
                ImageFrame? frame = null;

                List<byte[]> frameData = [];
                List<DateTime> timestamps = [];

                long streamPositionSlippage = (Reader.BaseStream.Position - Header.HeaderSize) % Header.FrameSize;

                if (streamPositionSlippage != 0)
                    Reader.BaseStream.Seek(Reader.BaseStream.Position - streamPositionSlippage + Header.FrameSize, SeekOrigin.Begin);


                try
                {
                    int frameID = Reader.ReadInt32();

                    for (int i = 0; i < Header.Planes; i++)
                    {
                        timestamps.Add(new DateTime(Reader.ReadInt64()));
                        frameData.Add(Reader.ReadBytes(Header.Width * Header.Height));
                    }

                    frame = new ImageFrame(frameData, PixelFormat.Gray8, timestamps, Header.UnitXSize, Header.UnitYSize, Header.UnitZSize, Header.Width, Header.Height, Header.Planes, true, frameID, Header.CameraSN, Header.CameraModel);
                }
                catch (Exception ex) { Console.WriteLine($"{ex.Message}"); }

                return frame;
            }
        }


        void CloseStream()
        {
            lock (readerAccessLock)
            {
                Reader?.Close();
                Reader?.Dispose();
                Reader = null;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    CloseStream();
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
