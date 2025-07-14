namespace Sardine.Devices.Hamamatsu.Camera
{
    public static class HamamatsuDataSource
    {
        public static HamamatsuSourceFrame? Source(HamamatsuCamera camera, out bool hasMore)
        {
            hasMore = false;
            if (camera.FrameBuffer is null)
                return null;

            try
            {
                camera.FrameBuffer.GetFrame(out HamamatsuSourceFrame? frame);
                
                if (frame is null)
                    return null;

                frame.UnitXSize = camera.PixelSize.Height / camera.Magnification;
                frame.UnitYSize = camera.PixelSize.Width / camera.Magnification;

                if (camera.FrameBuffer.FramesBehind > 0)
                    hasMore = true;

                return frame;
            }
            catch
            {
                return null;
            }

            
        }
    }
}
