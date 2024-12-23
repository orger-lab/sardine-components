using Sardine.Core;
using Sardine.Core.DataModel;
using System.Reflection;

namespace Sardine.Display
{
    public abstract partial class MultiSourceDisplay<K>
    {
        public static Vessel<H> GetVessel<H>(Func<H> displayGetter, IEnumerable<Vessel>? vesselsToDisplay = null, double framerate = 60) where H : MultiSourceDisplay<K>
        {
            var vessel = Freighter.Freight(displayGetter);

            vessel.Reload();

            vessel.AddSink<IDrawable<K>>(Display, vesselsToDisplay);

            var methodVessel = typeof(Vessel<H>).GetMethod(nameof(vessel.AddSink))!;
            var sinkSignatureType = typeof(Vessel<>.Sink<>);
            var methodDisplay = typeof(MultiSourceDisplay<K>).GetMethods(BindingFlags.Static | BindingFlags.NonPublic).Where((x) => x.Name == nameof(Display) && x.IsGenericMethod).First();
            foreach (Type t in vessel.Handle!.Converters.Keys)
            {
                methodVessel.MakeGenericMethod(t).Invoke(vessel, new object?[] { methodDisplay!.MakeGenericMethod(t).CreateDelegate(sinkSignatureType.MakeGenericType(typeof(H), t)), vesselsToDisplay });
            }

            vessel.SourceRate = framerate;

            

            return vessel;
        }



        protected static void Display<T>(MultiSourceDisplay<K> display, T data, MessageMetadata metadata)
        {
            Display(display, display.Converters[typeof(T)].Convert(data!), metadata);
        }


        static void Display(MultiSourceDisplay<K> display, IDrawable<K> data, MessageMetadata metadata)
        {
            int elementID = metadata.Sender;

            if (!display._typeToPositionMap.TryGetValue(elementID, out int id))
            {
                display.AddSignatureToDisplayBuffer(elementID);
                id = display._typeToPositionMap[elementID];
            }

            if (!data.IsAlive || (display._mostRecentDataIndex[id] is not null && metadata.SourceID < display._mostRecentDataIndex[id]))
                return;

            display.LastIsDrawn = false;

            display._mostRecentDataIndex[id] = metadata.SourceID;
            display._mostRecentData[id] = data;
            display._timeMap[id] = display.stopwatch.ElapsedTicks;

            if (display.Mode == DisplaySourceMode.ShowMostRecent)
            {
                display._nextToDraw[id] = data;
                return;
            }

            if (id == display._referenceDrawableID)
            {
                if (display._timeMap[display._referenceDrawableID] - display._lastFrameTime > display._ticksBetweenHalfFrames)
                {
                    display._onQueueToDraw[display._referenceDrawableID] = data;
                    for (int i = 0; i < display.ThingsToDrawCount; i++)
                    {
                        if (display._timeMap[display._referenceDrawableID] - display._timeMap[i] < display._ticksBetweenHalfFrames)
                            display._onQueueToDraw[i] = display._mostRecentData[i];
                    }
                }
                else
                {
                    display._nextToDraw[display._referenceDrawableID] = data;

                    for (int i = 0; i < display.ThingsToDrawCount; i++)
                    {
                        if (display._timeMap[display._referenceDrawableID] - display._timeMap[i] < display._ticksBetweenHalfFrames)
                            display._nextToDraw[i] = display._mostRecentData[i];
                    }
                }
                return;
            }

            if (display._timeMap[id] - display._timeMap[display._referenceDrawableID] < display._ticksBetweenHalfFrames)
            {
                if (display._timeMap[display._referenceDrawableID] - display._lastFrameTime < display._ticksBetweenHalfFrames)
                    display._nextToDraw[id] = data;
                else
                    display._onQueueToDraw[id] = data;
            }
        }
    }
}