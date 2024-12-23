using System.Drawing;
using System.Numerics;


namespace Sardine.Tracking.ZebraHRTailTracking
{
    public static class Utils
    {
        public static float[] FindCenterofMassOfArc(byte[] targetImage, Point imageSize, Vector2 startPoint, Vector2 startVector, float Angle, byte threshold)
        {
            float weight, weightedMass = 0, totalMass = 0;

            float[] centerOfMass = new float[2];

            Vector2 thisPoint = new Vector2();

            double floorX, floorY;

            float step = 2.0f * Angle / (float)Math.Ceiling(startVector.Length());

            for (float angle = -Angle; angle <= Angle; angle += step)
            {
                thisPoint = startPoint + RotateVectorRadians(startVector, angle);

                floorX = Math.Floor(thisPoint.X);
                floorY = Math.Floor(thisPoint.Y);

                if (floorX >= 0 && floorY >= 0 && (floorX + 1) < imageSize.X && (floorY + 1) < imageSize.Y)
                {
                    weight = BilinearInterpolate(targetImage, thisPoint, imageSize);

                    if (weight > threshold)
                    {
                        weightedMass += angle * weight;
                        totalMass += weight;
                    }
                }
            }
            centerOfMass[0] = totalMass;

            centerOfMass[1] = totalMass > 0 ? weightedMass / totalMass : 0;

            return centerOfMass;
        }

        public static float BilinearInterpolate(byte[] targetImage, Vector2 targetPoint, Point imageSize)
        {
            double floorX = Math.Floor(targetPoint.X);
            double floorY = Math.Floor(targetPoint.Y);
            double x = targetPoint.X - floorX;
            double y = targetPoint.Y - floorY;
            float f00 = targetImage[((int)floorX) + ((int)floorY) * imageSize.X];
            float f01 = targetImage[((int)floorX) + ((int)floorY + 1) * imageSize.X];
            float f10 = targetImage[((int)floorX + 1) + ((int)floorY) * imageSize.X];
            float f11 = targetImage[((int)floorX + 1) + ((int)floorY + 1) * imageSize.X];
            return (float)(f00 * (1 - x) * (1 - y) + f10 * (x) * (1 - y) + f01 * (1 - x) * (y) + f11 * x * y);
        }

        public static Vector2 RotateVectorRadians(Vector2 vector, float angle)
        {
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);

            return new Vector2()
            {
                X = (float)(vector.X * cos - vector.Y * sin),
                Y = (float)(vector.X * sin + vector.Y * cos)
            };
        }

        /// <summary>
        /// 2D or array circular buffer (or array of arrays) that implements a 'boxcar' averaging or filter function (?). Kind of moving average (?).   
        /// </summary>
        public class BoxcarArrayBuffer
        {
            int countArrays = 1; // counts the number of already added arrays
            double dcount;
            double[][] buffer2D; // double[,] buffer can be used instead
            double[] currentAv; // current average or output of the boxcar function given previous arrays in the 2D buffer.
            int nArrays;
            int arraySize;

            /// <summary>
            /// Initializes a 2D buffer double['length']['numElements'].
            /// </summary>
            /// <param name="numElements"></param>
            /// <param name="length"></param>
            public BoxcarArrayBuffer(int numElements, int length)
            {
                this.nArrays = length;
                this.arraySize = numElements;

                buffer2D = new double[nArrays][];
                for (int i = 0; i < nArrays; i++)
                {
                    buffer2D[i] = new double[numElements];
                }
                currentAv = new double[numElements];
            }


            public double[] AddElements(double[] newElements)
            {
                int i;

                /* If buffer not yet full do:            
                    currentAv[i] = newElements[i] 
                    currentAv[i] = currentAv[i] * 1/2 + newElements[i] * 1/2
                    currentAv[i] = currentAv[i] * 2/3 + newElements[i] * 1/3
                    ...
                    currentAv[i] = currentAv[i] * (nArrays-1)/nArrays + newElements[i] * 1/nArrays
                 * */
                if (countArrays <= nArrays)
                {
                    dcount = (double)countArrays;

                    for (i = 0; i < arraySize; i++)
                    {
                        currentAv[i] = currentAv[i] * (dcount - 1.0) / dcount + newElements[i] / dcount;
                    }
                }

                /**After buffer is full do:             
                    currentAv[i] =  currentAv[i] - buffer2D[1][i] + newElements[i] / nArrays;
                    currentAv[i] =  currentAv[i] - buffer2D[2][i] + newElements[i] / nArrays;
                    ...
                    currentAv[i] =  currentAv[i] - buffer2D[countArrays % nArrays][i] + newElements[i] / nArrays;
                 * 
                 * Note that buffer[i] is divided by nArrays at the time of saving to buffer (last for loop below) - could be simplified.
                **/
                else
                {
                    for (i = 0; i < arraySize; i++)
                    {
                        currentAv[i] -= buffer2D[countArrays % nArrays][i];
                        currentAv[i] += newElements[i] / (double)nArrays;
                    }
                }

                /** Save newElements (new array) to buffer (circularly) performing:
                    buffer2D[1][i] = newElements[i] / nArrays;
                    buffer2D[2][i] = newElements[i] / nArrays;
                    ...
                    buffer2D[countArrays % nArrays][i] = newElements[i] / nArrays;
                 * 
                 * Basically, new array is saved to buffer always divided by the length of the buffer.
                **/
                for (i = 0; i < arraySize; i++)
                {
                    buffer2D[countArrays % nArrays][i] = newElements[i] / (double)nArrays;
                }

                countArrays++;

                /* We end up (after buffer is full) with an operation like:
                 * currentAv =  prevAv + (newElements - bufElements[bufIter]) / nArrays; ----> Similar to a moving average. 'boxcar' maybe not best name (?)
                 * */
                return currentAv;
            }

            public double[] AddElements(float[] newElements)
            {
                int i;

                if (countArrays <= nArrays)
                {
                    dcount = (double)countArrays;

                    for (i = 0; i < arraySize; i++)
                    {
                        currentAv[i] = currentAv[i] * (dcount - 1.0) / dcount + (double)newElements[i] / dcount;
                    }
                }
                else
                {
                    for (i = 0; i < arraySize; i++)
                    {
                        currentAv[i] -= buffer2D[countArrays % nArrays][i];
                        currentAv[i] += (double)newElements[i] / nArrays;
                    }
                }
                for (i = 0; i < arraySize; i++)
                {
                    buffer2D[countArrays % nArrays][i] = (double)newElements[i] / nArrays;
                }
                countArrays++;
                return currentAv;
            }
        }
    }
}
