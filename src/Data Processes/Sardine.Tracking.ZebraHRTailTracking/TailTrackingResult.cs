using Sardine.Recording.Data.Text;

using System.Numerics;

namespace Sardine.Tracking.ZebraHRTailTracking
{
    public abstract class TailTrackingResult : ITextWritable
    {

        public Vector2[] tailPoints;
        public double[] predictors;
        public float[] tailAngles;
        public float[] tailValues;
        public readonly uint imageWidth;
        public readonly uint imageHeight;
        public int ID;

        public bool IsAlive => true;

        public TailTrackingResult(int id, Vector2[] tailPoints, double[] predictors, float[] tailAngles, float[] tailValues, uint imageWidth, uint imageHeight)
        {
            ID = id;
            this.tailPoints = tailPoints;
            this.predictors = predictors;
            this.tailAngles = tailAngles;
            this.tailValues = tailValues;
            this.imageWidth = imageWidth;
            this.imageHeight = imageHeight;
        }

        public string WriteHeader()
        {
            string[] text = new string[1 + 32 + 16]; //ID + TAILPOINTS + TAIL ANGLES

            text[0] = "FrameID";

            for (int i = 1, j = 0; i < 33; i += 2, j++)
            {
                text[i] = "x" + j;
                text[i + 1] = "y" + j;
            }

            for (int i = 0; i < 16; i++)
            {
                text[33 + i] = "angle" + i;
            }

            return string.Join(" ", text);

        }

        public string WriteData()
        {
            string[] text = new string[1 + 32 + 16]; //ID + TAILPOINTS + TAIL ANGLES

            text[0] = ID.ToString();

            for (int i = 1, j = 0; i < 33; i += 2, j++)
            {
                text[i] = tailPoints[j].X.ToString();
                text[i + 1] = tailPoints[j].Y.ToString();
            }

            for (int i = 0; i < 16; i++)
            {
                text[33 + i] = tailAngles[i].ToString();
            }

            return string.Join(" ", text);
        }

    }

    public class MPTailTrackingResult : TailTrackingResult
    {
        public MPTailTrackingResult(int id, Vector2[] tailPoints, double[] predictors, float[] tailAngles, float[] tailValues, uint imageWidth, uint imageHeight) :
            base(id, tailPoints, predictors, tailAngles, tailValues, imageWidth, imageHeight)
        { }
    }
}
