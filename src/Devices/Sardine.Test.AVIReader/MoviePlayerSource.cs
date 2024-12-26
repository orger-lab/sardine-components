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
    public static class MoviePlayerSource
    {
        public static IImageFrame? SourceFrame(MoviePlayer player, out bool hasMore)
        {
            hasMore = false;

            (int frameID, byte[] frameData) = player.GetNextFrame();


            return new ImageFrame([frameData], PixelFormat.Gray8, [DateTime.Now], 1, 1, 1, player.Width, player.Height, 1, true, frameID, "AVI Player", "Test Player");

        }
    }
}
