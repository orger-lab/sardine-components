using OpenCV.Net;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;

namespace Sardine.Test.AVIReader
{
    public class AVIReader
    {
        public bool AddNoise { get; set; }
        public int OutputFrameCount { get; internal set; } = 0;

        public int Height { get; } = 0;
        public int Width { get; } = 0;

        public IReadOnlyList<byte[]> AllFrames { get; }

        public AVIReader(string path)
        {
            Capture stream = Capture.CreateFileCapture(path);

            List<byte[]> frames = new List<byte[]>();

            while (true)
            {

                IplImage frame = stream.QueryFrame();
                if (frame is null)
                    break;

                int size_row_raw = frame.Width;
                int rem = size_row_raw % 4;
                int width_step = size_row_raw + rem;

                if (Height == 0) Height = frame.Height;
                if (Width == 0) Width = width_step;

                var imageSize = new Size(frame.Width, frame.Height);

                IplImage singleColorFrame = new IplImage(imageSize, IplDepth.U8, 1);

                CV.Split(frame, singleColorFrame, null, null, null);

                byte[] imageData = new byte[(Width) * Height];

                Marshal.Copy(singleColorFrame.ImageData, imageData, 0, frame.Width * frame.Height);
                
                frame.Close();
                singleColorFrame.Close();

                frames.Add(imageData);
            }

            stream.Close();
            AllFrames = frames;

        }
    }
}
