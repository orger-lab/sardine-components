using Sardine.Core;
using Sardine.Core.DataModel;
using Sardine.Recording.Data.Text;
using Sardine.Test.FictiveWorkload;
using Sardine.Test.Timestamper;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Reflection.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StressTest
{
    public class BusyWorker : Fleet
    {
        public Vessel<BusyLoad> FictiveWorkload { get; }
        public Vessel<TimestampAppender> TimestampBefore { get; }
        public Vessel<TimestampAppender> TimestampAfter { get; }
        public Vessel<TimestampDifferenceCalculator> MessageTimer { get; }
        public Vessel<TextMessageRecorder> OutputWriter { get; }
        public Vessel<MockInput> InputGenerator { get; }

        BenchmarkTimer benchmarkTimer;

        public BusyWorker()
        {
            int sourceRate = 50;
            InputGenerator = Freighter.Freight<MockInput>(() => new MockInput(sourceRate), initializer: (mockInput) => { mockInput.Start(); InputGenerator!.IsActive = true; mockInput.DataReady += (_, _) => InputGenerator!.GenerateData(); });
            InputGenerator.SourceRate = sourceRate;
            InputGenerator.AddSource<MockData>(MockInput.Source);


            TimestampBefore = Freighter.Freight(() => new TimestampAppender(), initializer: (_) => TimestampBefore.IsActive = true);
            TimestampBefore.AddTransformer<MockData, TimestampedContainer<MockData>>(TimestampAppender.Transform, [InputGenerator]);

            FictiveWorkload = Freighter.Freight(() => new BusyLoad() { BusyCycles = 10000, BusyDifficulty = 1, NumThreads = 12 }, initializer: (_) => FictiveWorkload.IsActive = true);
            FictiveWorkload.AddTransformer<TimestampedContainer<MockData>, TimestampedContainer<MockData>>(BusyLoadTransformer<TimestampedContainer<MockData>>.WorkMultithreaded, [TimestampBefore]);

            TimestampAfter = Freighter.Freight(() => new TimestampAppender(), initializer: (_) => TimestampAfter.IsActive = true);
            TimestampAfter.AddTransformer<TimestampedContainer<MockData>, TimestampedContainer<TimestampedContainer<MockData>>>(TimestampAppender.Transform, [FictiveWorkload]);

            MessageTimer = Freighter.Freight(() => new TimestampDifferenceCalculator(), initializer: (_) => MessageTimer.IsActive = true);
            MessageTimer.AddTransformer<TimestampedContainer<TimestampedContainer<MockData>>, TimestampDifference>(TimestampDifferenceCalculator.Transform, [TimestampAfter]);

            OutputWriter = Freighter.Freight(() => new TextMessageRecorder() { Path = "D:\\", FileName = $"busyWorkerLatency_{sourceRate:0.00}.txt" }, initializer: (_) => OutputWriter.IsActive = true);
            OutputWriter.AddSink<ITextWritable>(TextMessageRecorder.Sink);

            benchmarkTimer = new BenchmarkTimer(this);
            benchmarkTimer.Start();

        }
    }

    public class BenchmarkTimer
    {
        Thread workerThread;
        BusyWorker worker;
        public BenchmarkTimer(BusyWorker worker)
        {
            workerThread = new Thread(SetTimedParameters);
            this.worker = worker;
        }

        public void SetTimedParameters()
        {
            Console.WriteLine("Awaiting relaxed start ..");
            Thread.Sleep(20000);

            Console.WriteLine("Start timed benchmark");
            Console.WriteLine($"Current framerate: {worker.InputGenerator.SourceRate:0.00}");
            Console.WriteLine("-------------------------");
            Console.WriteLine("-------------------------");

            for (int i = 0; i < 20; i++)
            {
                Console.WriteLine($"Set difficulty: {i + 1}");
                worker.FictiveWorkload.ExecuteCall((BusyLoad load) => { load.BusyDifficulty = (uint)(i + 1); });
                Thread.Sleep(10000);
            }


            Console.WriteLine("Finishing");
            Environment.Exit(0);
        }

        public void Start()
        {
            workerThread.Start();
        }
    }

    public class MockInput
    {
        Random RandomSource { get; }

        public double SourceRate { get; }
        DateTime? StartTimestamp { get; set; }
        public int ReturnedValues { get; private set; }

        public int CurrentValue => (int)Math.Floor((double)stopwatch.ElapsedTicks / tickRate);

        public int tickRate = 10000 * 1000;
        Stopwatch stopwatch = new Stopwatch();

        public event EventHandler? DataReady;

        public MockInput(double sourceRate)
        {
            SourceRate = sourceRate;
            RandomSource = new Random();

            tickRate = (int)(1000 * 10000 / (double)sourceRate);
        }

        public void Start()
        {
            stopwatch.Start();
        }

        //private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        //{
        //    CurrentValue++;
        //    DataReady?.Invoke(this, EventArgs.Empty);
        //}

        public int GetValue()
        {
            StartTimestamp ??= DateTime.UtcNow;
            ReturnedValues++;
            return RandomSource.Next();
        }

        public static MockData? Source(MockInput input, out bool hasMore)
        {
            int mockValue = input.GetValue();

            hasMore = (input.ReturnedValues < input.CurrentValue);

            //Console.WriteLine($"{input.ReturnedValues} {input.CurrentValue}");

            return new MockData(mockValue);


            //if ((DateTime.UtcNow-input.StartTimestamp)!.Value.TotalSeconds/input.ExpectedSourceRate > (input.ReturnedValues + 1))
            //    hasMore = true;

            
        }
    }

    public class MockData : ITimestampedObject, IValueContainer
    {
        public int Value { get; set; }
        public DateTime Timestamp { get; }

        public MockData(int value)
        {
            Value = value;
            Timestamp = DateTime.UtcNow;
        }
    }

    public class BusyWorkerNotLoaded : IDisposable
    {
        MockInput InputGenerator;
        TimestampAppender TimestampBefore;
        BusyLoad FictiveWorkload;
        TimestampAppender TimestampAfter;
        TimestampDifferenceCalculator MessageTimer;
        TextMessageRecorder OutputWriter;
        Thread timerThread;
        System.Timers.Timer inputGeneratorClock;
        long id = 0;

        object idLock = new();
        object writerLock = new();

        float sourceRate;

        public MessageMetadata ProduceMetadataObject(long id)
        {
            OnSentDataEventArgs thisData = new OnSentDataEventArgs(new object(), [], 0, string.Empty, 0, 0, id);
            return thisData.Metadata;
        }

        public void Start()
        {
            timerThread.Start();
            inputGeneratorClock.Enabled = true;
        }        

        public void Wait()
        {
            timerThread.Join();
        }


        public BusyWorkerNotLoaded(float sourceRate)
        {
            this.sourceRate = sourceRate;
            
            InputGenerator = new MockInput(sourceRate);
            TimestampBefore = new TimestampAppender();
            FictiveWorkload = new BusyLoad() { BusyCycles = 40250, BusyDifficulty = 1, NumThreads = 12 };
            TimestampAfter = new TimestampAppender();
            MessageTimer = new TimestampDifferenceCalculator();
            OutputWriter = new TextMessageRecorder() { Path = "D:\\", FileName = $"busyWorkerLatency_NoSardine_{sourceRate:0.00}_" };

            inputGeneratorClock = new System.Timers.Timer(1000 / 50);
            inputGeneratorClock.Elapsed += InputGeneratorClock_Elapsed;


            
            timerThread = new Thread(SetTimedParameters);

            
        }

        private async void InputGeneratorClock_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (InputGenerator.ReturnedValues < InputGenerator.CurrentValue)
                await AdvanceInput();
        }

        SemaphoreSlim _semaphoreWrite = new SemaphoreSlim(1, 1);
        SemaphoreSlim _semaphoreID = new SemaphoreSlim(1, 1);
        SemaphoreSlim _semaphoreWork = new SemaphoreSlim(1, 1);

        


        async Task AdvanceInput()
        {
        
            MockData? dataOut = MockInput.Source(InputGenerator, out bool hasMore);

            if (dataOut is null)
                return;

            MessageMetadata metadata;

            await _semaphoreID.WaitAsync();
            id++;
            metadata = ProduceMetadataObject(id);
            _semaphoreID.Release();

            if (hasMore)
            {
                await AdvanceInput();
            }

            await Task.Run(async () =>
                {
                    TimestampedContainer<MockData> timestampedBefore = TimestampAppender.Transform(TimestampBefore, dataOut, metadata);
                    await _semaphoreWork.WaitAsync();
                    TimestampedContainer<MockData> afterFictiveWork = BusyLoadTransformer<TimestampedContainer<MockData>>.WorkMultithreaded(FictiveWorkload, timestampedBefore, metadata)!;
                    _semaphoreWork.Release();
                    
                    TimestampedContainer<TimestampedContainer<MockData>> timestampedAfter = TimestampAppender.Transform(TimestampAfter, afterFictiveWork, metadata);
                    TimestampDifference tsDifference = TimestampDifferenceCalculator.Transform(MessageTimer, timestampedAfter, metadata);

                    await _semaphoreWrite.WaitAsync();
                    TextMessageRecorder.Sink(OutputWriter, tsDifference, metadata);
                    _semaphoreWrite.Release();

                });
        }
        
            
        public void SetTimedParameters()
        {
            Console.WriteLine("Awaiting relaxed start ..");
            InputGenerator.Start();
            Thread.Sleep(20000);
                        

            Console.WriteLine("Start timed benchmark");
            Console.WriteLine($"Current framerate: {sourceRate:0.00}");
            Console.WriteLine("-------------------------");
            Console.WriteLine("-------------------------");

            for (int i = 0; i < 20; i++)
            {
                Console.WriteLine($"Set difficulty: {i + 1}");
                FictiveWorkload.BusyDifficulty = (uint)(i + 1);
                Thread.Sleep(10000);
            }


            Console.WriteLine("Finishing");
            Environment.Exit(0);
        }

        ~BusyWorkerNotLoaded()
        {
            OutputWriter.CloseStreams();
        }

        public void Dispose()
        {
            OutputWriter.Dispose();
        }
    }
}
