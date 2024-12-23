using System.Drawing;
using System.Numerics;

namespace Sardine.Tracking.ZebraHRTailTracking
{
    public partial class MPTailTracking
    {

        public byte BodySearchThreshold
        {
            get; set;
        } = 1;



        public uint StartX
        {
            get => startX; set
            {
                startX = value;
                UpdateSearchRadius();
            }
        }
        public uint StartY
        {
            get => startY; set
            {
                startY = value;
                UpdateSearchRadius();
            }
        }
        public uint EndX { get => endX; set {
                endX = value;
                UpdateSearchRadius();
            } }
        public uint EndY { get => endY; set {
                endY = value;
                UpdateSearchRadius();
            }
        }

        public Vector2 BodyVector
        {
            get; protected set;
        }

        private void UpdateSearchRadius()
        {
            BodyVector = new Vector2(EndX - (int)StartX, EndY - (int)StartY);
            _tailSearchRadius = BodyVector.Length() / (NUMSEGMENTS - 1);
        }

        public MPTailTracking()
        {
            UpdateSearchRadius();
        }

        // tracking static settings
        private const int NUMSEGMENTS = 16;
        private const float TAILSEARCHANGLE = (float)Math.PI / 2;

        private float _tailSearchRadius;

        //private static readonly int _velSegments = (int)Math.Round(NUMSEGMENTS * 8.0 / 10.0);
        private Utils.BoxcarArrayBuffer _angleBuffer = new Utils.BoxcarArrayBuffer(NUMSEGMENTS, 200);

        private double[] _prevTailAngles = new double[12];

        Vector2[] tailPoints = new Vector2[NUMSEGMENTS];

        double[] predictors = new double[24];
        double[]? bufferedAngles;

        float[] tailAngles = new float[NUMSEGMENTS];
        float[] tailValues = new float[NUMSEGMENTS];

        float[] threeSweeps = new float[2];
        float[] nextSweep = new float[2];
        private uint startX = 40;
        private uint startY = 0;
        private uint endX = 40;
        private uint endY = 60;

        void CheckAndFixCoordinateSystem(uint width, uint height)
        {
            if (StartX >= width)
                StartX = width - 1;

            if (EndX >= width)
                EndX = width - 1;

            if (StartY >= height)
                StartY = height - 1;

            if (EndY >= height)
                EndY = height - 1;
        }

        protected MPTailTrackingResult GetTailPositionFromImage(byte[] frameData, uint width, uint height, int id)
        {

            CheckAndFixCoordinateSystem(width, height);
            tailPoints[0] = new Vector2(StartX, StartY);

            Point point = new((int)width, (int)height);
            Vector2 nextVector = Vector2.Normalize(BodyVector);

            // Find points
            for (int i = 0; i < NUMSEGMENTS - 1; i++)
            {
                //run 3 radial sweeps 1 pixel either side of the preferred search radius

                threeSweeps[0] = 0.0f;
                threeSweeps[1] = 0.0f;
                for (int j = -1; j < 2; j++)
                {
                    nextSweep = Utils.FindCenterofMassOfArc(
                                                frameData,
                                                point,
                                                tailPoints[i],
                                                nextVector * (_tailSearchRadius + j),
                                                TAILSEARCHANGLE,
                                                BodySearchThreshold);

                    threeSweeps[0] += nextSweep[0];
                    threeSweeps[1] += nextSweep[1];
                }


                //record the total pixel value found and the mean angle
                //tailValues[i] = threeSweeps[0];
                tailAngles[i] = threeSweeps[1] / 3.0f;


                //start the next search from the new point in the new direction
                nextVector = Utils.RotateVectorRadians(nextVector, tailAngles[i]);
                tailPoints[i + 1] = tailPoints[i] + (nextVector * _tailSearchRadius);
            }
        
            bufferedAngles = _angleBuffer.AddElements(tailAngles); //averaged tail angles

            //remove average from actual estimated angles. Use only 12 of the 16 points.
            predictors[0] = tailAngles[0] - bufferedAngles[0];

            for (int i = 1; i < 12; i++)
                predictors[i] = predictors[i - 1] + tailAngles[i] - bufferedAngles[i];

            for (int i = 0; i < 12; i++)
            {
                predictors[i + 12] = tailAngles[i] - _prevTailAngles[i];
                _prevTailAngles[i] = tailAngles[i];
            }

            return new MPTailTrackingResult(id, tailPoints, predictors, tailAngles, tailValues, width, height);
        }
    }
}
