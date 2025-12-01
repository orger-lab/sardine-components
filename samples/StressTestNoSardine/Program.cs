using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.CodeAnalysis;
using Sardine.Core.DataModel;
using Sardine.Recording.Data.Text;
using Sardine.Test.FictiveWorkload;
using Sardine.Test.Timestamper;
using StressTest;
using System.Data;

namespace StressTestNoSardine
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<SortBenchmark>();



            return;

            BusyWorkerNotLoaded worker = new BusyWorkerNotLoaded(20);

            worker.Start();
            worker.Wait();

            worker.Dispose();
        }


    }

public class SortBenchmark
    {
        [Params(1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20)]  // Size of the array
        public uint N;


        BusyLoad load;
        private MessageMetadata metadata;

        public MockInput InputGenerator { get; private set; }
        public TimestampAppender TimestampBefore { get; private set; }

        // Setup method to create an array before each benchmark
        [GlobalSetup]
        public void Setup()
        {
            load = new BusyLoad() { BusyCycles = 10000, BusyDifficulty = N, NumThreads = 12 };
            InputGenerator = new MockInput(1);
        }
        public MessageMetadata ProduceMetadataObject(long id)
        {
            OnSentDataEventArgs thisData = new OnSentDataEventArgs(new object(), [], 0, string.Empty, 0, 0, id);
            return thisData.Metadata;
        }

        [Benchmark]
        public void DoWork()
        {
            metadata = ProduceMetadataObject(1);
            MockData? dataOut = MockInput.Source(InputGenerator, out bool hasMore);
            TimestampedContainer<MockData> timestampedBefore = TimestampAppender.Transform(TimestampBefore, dataOut, metadata);
            TimestampedContainer<MockData> afterFictiveWork = BusyLoadTransformer<TimestampedContainer<MockData>>.WorkMultithreaded(load, timestampedBefore, metadata)!;
        }
    }

}
