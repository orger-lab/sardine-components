using OpenCV.Net;
using System.Runtime.InteropServices;

namespace Sardine.Test.AVIReader
{
    public class AVIReader
    {
        public AVIReader() { }

        public MoviePlayer GetPlayer(string path)
        {
            int height = 0;
            int width = 0;

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

                if (height == 0) height = frame.Height;
                if (width == 0) width = width_step;

                var imageSize = new Size(frame.Width, frame.Height);

                IplImage singleColorFrame = new IplImage(imageSize, IplDepth.U8, 1);

                CV.Split(frame, singleColorFrame, null, null, null);

                byte[] imageData = new byte[(width) * height];

                Marshal.Copy(singleColorFrame.ImageData, imageData, 0, frame.Width * frame.Height);

                frame.Close();
                singleColorFrame.Close();

                frames.Add(imageData);
            }

            stream.Close();

            return new MoviePlayer(frames, width, height);
        }
    }
}
