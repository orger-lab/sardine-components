namespace Sardine.Test.AVIReader
{
    public class MoviePlayer
    {
        public int Height { get; } = 0;
        public int Width { get; } = 0;

        public int FrameCaret { get; private set; } = -1;
        private IReadOnlyList<byte[]> AllFrames { get; }

        internal MoviePlayer(IList<byte[]> frameData, int width, int height)
        {
            Width = width;
            Height = height;

            AllFrames = (IReadOnlyList<byte[]>)frameData;
        }

        public (int,byte[]) GetNextFrame()
        {
            FrameCaret++;
            return (FrameCaret, AllFrames[FrameCaret % AllFrames.Count]);
        }
    }
}
