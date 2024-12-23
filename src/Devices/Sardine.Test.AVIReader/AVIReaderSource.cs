using OpenCV.Net;
using Sardine.ImageProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sardine.Test.AVIReader
{
    public static class AVIReaderSource
    {
        public static IImageFrame? SourceFrame(AVIReader reader, out bool hasMore)
        {
            hasMore = false;



            return new ImageFrame([reader.AllFrames[reader.OutputFrameCount % reader.AllFrames.Count]], PixelFormat.Gray8, [DateTime.Now], 1, 1, 1, reader.Width, reader.Height, 1, true, reader.OutputFrameCount++, "AVI Reader", "Test Reader");

            // return new ImageFrame([imageData], PixelFormat.Gray8, [DateTime.Now], 1,1, 1, width+2, height, 1, true, reader.OutputFrameCount++, "AVI Reader", "Test Reader");
        }
    }
}
