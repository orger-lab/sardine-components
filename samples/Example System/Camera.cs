namespace ExampleSystem
{
    public class Camera
    {
        public string SerialNumber { get; }
        public int FrameRate
        {
            get => frameRate;
            set
            {
                frameRate = value;
                frameGenerationTimer.Interval = 1000 / FrameRate;
            }
        }

        public int Width { get; }
        public int Height { get; }

        public bool IsRunning { get; private set; }

        private Random randomGenerator;

        private int framesBehind = 0; // This simulates a camera buffer

        private System.Timers.Timer frameGenerationTimer = new System.Timers.Timer(); // This simulates frame generation 
        private int frameRate;

        internal Camera(string serialNumber, int frameRate, int width, int height)
        {
            SerialNumber = serialNumber;
            FrameRate = frameRate;
            Width = width;
            Height = height;
            randomGenerator = new Random();
            frameGenerationTimer.Elapsed += FrameGenerationTimer_Elapsed;
            frameGenerationTimer.Interval = 1000 / frameRate;
        }

        public void Start() { IsRunning = true; frameGenerationTimer.Enabled = true; }
        public void Stop()  { IsRunning = false; frameGenerationTimer.Enabled = false; framesBehind = 0; }

        public void BreakCamera()
        {
            throw new Exception("An error was thrown!");
        }

        public CameraFrame? GetNextFrame()
        {
            if (!IsRunning)
                return null;

            if (framesBehind <= 0)
                return null;

            byte[] data = new byte[Width * Height];

            for (int i = 0; i < Width*Height; i++)
            {
                data[i] = (byte)randomGenerator.Next(255);
            }

            framesBehind--;

            return new CameraFrame(data) { Width = Width, Height = Height };
        }

        private void FrameGenerationTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            framesBehind++;
        }
    }

}
