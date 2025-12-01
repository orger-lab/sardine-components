
using Sardine.Core.DataModel;
using Sardine.Test.Timestamper;


namespace Sardine.Test.FictiveWorkload
{
    public class BusyLoad
    {
        private uint numThreads = 12;
        private uint busyDifficulty = 3;
        private ulong busyCycles = 40250;

        public ulong BusyCycles
        {
            get => busyCycles;
            set
            {
                busyCycles = Math.Max(1, value);
            }
        }
        public uint BusyDifficulty
        {
            get => busyDifficulty;
            set
            {
                busyDifficulty = Math.Max(1, Math.Min(20, value));
            }
        }
        public uint NumThreads
        {
            get => numThreads;
            set
            {
                numThreads = Math.Max(1, Math.Min(value, MaxThreads));
            }
        }

        public uint MaxThreads { get; }
        public Random RandEngine { get; }

        public BusyLoad()
        {
            MaxThreads = (uint)Environment.ProcessorCount;
            RandEngine = new Random();
        }



        public double WorkThreads(uint numThreads, int id)
        {
            if (numThreads > MaxThreads)
                throw new ArgumentOutOfRangeException(nameof(numThreads));

            CountdownEvent countdown = new CountdownEvent((int)numThreads);

            double valueFinal = 0;
            for (int i = 0; i < numThreads; i++)
            {
                ThreadPool.QueueUserWorkItem(_ => 
                {
                    valueFinal += Work(); 
                    countdown.Signal();
                });               
            }

            countdown.Wait();

            return valueFinal;
        }


        public double Work()
        {
            // var watch = System.Diagnostics.Stopwatch.StartNew();
            float valueFinal = 0;
            for (ulong i = 0; i < BusyCycles; i++)
            {
                valueFinal += ComputeSmallFactorial(BusyDifficulty);
            }
            // Console.WriteLine(watch.ElapsedMilliseconds);
            return valueFinal;
        }

        private ulong ComputeSmallFactorial(uint value)
        {
            if (value == 0)
                return 1;

            return ComputeSmallFactorial(value - 1) * (ulong)Math.Ceiling(value*(1-RandEngine.NextDouble()*0.05));
        }
    }

    public static class BusyLoadTransformer<T>
    {
        public static T? Work(BusyLoad loadObject, T dataIn, MessageMetadata metadata)
        {
            loadObject.Work();

            return dataIn;
        }

        public static T? WorkMultithreaded<T>(BusyLoad loadObject, T dataIn, MessageMetadata metadata) where T : IValueContainer
        {
            double valueOut = loadObject.WorkThreads(loadObject.NumThreads, dataIn.Value);

            dataIn.Value = (int)Math.Ceiling(Math.Log10(valueOut));
            return dataIn;
        }
    }

}
