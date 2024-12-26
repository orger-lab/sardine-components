namespace ExampleSystem
{
    public class CameraFrame
    {
        public byte[] Data { get; }
        public int Width { get; init; }
        public int Height { get; init; }

        public CameraFrame(byte[] data)
        {
            Data = data;
        }
    }
}
