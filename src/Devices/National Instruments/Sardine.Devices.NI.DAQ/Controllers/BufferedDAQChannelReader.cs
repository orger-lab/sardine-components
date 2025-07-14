using NationalInstruments.DAQmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Sardine.Devices.NI.DAQ.Controllers
{
    public partial class BufferedDAQChannelReader : IDisposable
    {
        private readonly AnalogMultiChannelReader? _daqAnalogReader;
        private readonly DigitalMultiChannelReader? _daqDigitalReader;
        private readonly CounterSingleChannelReader[] _daqCounterReaders;


        private double[,]? _dataBinAnalog;
        private int[][] _dataBinCounter;

        private readonly AsyncCallback? callbackAnalog;
        private readonly AsyncCallback? callbackDigital;
        private readonly AsyncCallback[] callbackCounters;

        long analogIDCounter = 0;
        long digitalIDCounter = 0;
        long[] counterIDCounter = []; 

        internal class AnalogReadout : DAQDevice
        {
            public AnalogReadout(DAQBoard board, string name, IEnumerable<DaqPhysicalChannelID> analogChannelsToRead)
                          : base(board, name, null, analogChannelsToRead)
            { }

            public override void SetupChannels()
            {
                foreach (var channel in MainTask.Channels)
                {
                    MainTask.AIChannels.CreateVoltageChannel(channel.ToString(), $"{channel.FriendlyName}_{MainTask.Name}_BufferedAnalogReadout", NationalInstruments.DAQmx.AITerminalConfiguration.Rse, -10, 10, AIVoltageUnits.Volts);
                }
            }
        }

        internal class CounterReadout : DAQDevice
        {
            public CounterReadout(DAQBoard board, string name, DaqPFI pfi, int counterID, CounterEdge edge) : base(board, name, (pfi, counterID, edge), []) { }

            public override void SetupChannels()
            {
                var counterToRead = ((DaqPFI PFI, int CounterID, CounterEdge Edge))ChannelParameters!;

                MainTask.CIChannels.CreateCountEdgesChannel(MainTask.Board.IO.CounterOut[counterToRead.CounterID].ToString(),
                                                            $"PFI{counterToRead.PFI.PFI}_{MainTask.Name}_BufferedCounterReadout",
                                                            counterToRead.Edge == CounterEdge.Rising ? CICountEdgesActiveEdge.Rising : CICountEdgesActiveEdge.Falling,
                                                            0, CICountEdgesCountDirection.Up);
                MainTask.CIChannels[0].CountEdgesTerminal = counterToRead.PFI.ToString();
            }
        }

        internal class DigitalReadout : DAQDevice
        {
            public DigitalReadout(DAQBoard board, string name, IEnumerable<DaqPhysicalChannelID> digitalChannelsToRead)
                           : base(board, name, null, digitalChannelsToRead)
            { }

            public override void SetupChannels()
            {
                foreach (var channel in MainTask.Channels)
                {
                    MainTask.DIChannels.CreateChannel(channel.ToString(), $"{channel.FriendlyName}_{MainTask.Name}_BufferedDigitalReadout", ChannelLineGrouping.OneChannelForEachLine);
                }
            }
        }

        private readonly AnalogReadout? analogTask;
        private readonly DigitalReadout? digitalTask;
        private readonly CounterReadout[] counterReadouts;
        readonly int samplesPerReadCycle;
        public DAQSample[] DataBuffer { get; private set; }

        private readonly SampleSynchronizerCollection syncCollection;
        private readonly object _caretLock = new();

        public ManualResetEvent SamplesBehindExist { get; } = new ManualResetEvent(false);


        private long samplesBehind=0;
        public long SamplesBehind
        {
            get => samplesBehind;
            private set
            {
                samplesBehind = value;
                if (value == -1 && ExceptionOnOverflow)
                    throw new BufferOverflowException(this);

                if (value == 0)
                    SamplesBehindExist.Reset();
                else
                    SamplesBehindExist.Set();
            }
        }

        private long writeCaret = -1;
        public long WriteCaret
        {
            get => writeCaret;
            set
            {
                lock (_caretLock)
                {
                    writeCaret = value;
                    if (SamplesBehind >= 0)
                        SamplesBehind = WriteCaret - ReadCaret > DataBuffer.Length ? -1 : WriteCaret - ReadCaret;
                }
            }
        }

        private long readCaret = -1;
        public long ReadCaret
        {
            get => readCaret;
            set
            {
                lock (_caretLock)
                {
                    readCaret = value;
                    
                    if (SamplesBehind >= 0)
                        SamplesBehind = WriteCaret - ReadCaret;
                }
            }
        }

        private DAQSample lastSample;
        readonly private object readLock = new();

        public DAQSample LastSample
        {
            get => lastSample;
            private set
            {
                lastSample = value;
                WriteCaret++;
                DataBuffer[WriteCaret % DataBuffer.Length] = lastSample;
                NewSampleAvailable?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler? NewSampleAvailable;

        public bool ExceptionOnOverflow { get; set; } = true;


        public BufferedDAQChannelReader(DAQBoard board, string name, DaqPhysicalChannelID device, long bufferSize, int samplesPerReadCycle = 1)
                                 : this(board, name, [device], [],  bufferSize, samplesPerReadCycle)
        { }

        public BufferedDAQChannelReader(DAQBoard board, string name, int counterID, DaqPFI counter, long bufferSize, int samplesPerReadCycle = 1, CounterEdge counterEdge = CounterEdge.Rising )
                                 : this(board, name, [], [(counterID, counter, counterEdge)], bufferSize, samplesPerReadCycle)
        { }

        public BufferedDAQChannelReader(DAQBoard board, string name, IEnumerable<DaqPhysicalChannelID> devices, IEnumerable<(int, DaqPFI, CounterEdge)> counters, long bufferSize, int samplesPerReadCycle = 1)
        {
            int _digitalCounter = 0;
            int _analogCounter = 0;

            counterReadouts = new CounterReadout[counters.Count()];
            callbackCounters = new AsyncCallback[counters.Count()];
            _daqCounterReaders = new CounterSingleChannelReader[counters.Count()];
            _dataBinCounter = new int[counters.Count()][];

            foreach (DaqPhysicalChannelID entry in devices)
            {
                switch (entry.ChannelType)
                {
                    case PhysicalChannelTypes.AI:
                        _analogCounter++;
                        break;
                    case PhysicalChannelTypes.DILine:
                        _digitalCounter++;
                        break;
                    default:
                        throw new ArgumentException($"{entry.FriendlyName} can't be read", nameof(devices));
                }
            }

            if (_analogCounter > 0)
            {
                analogTask = new AnalogReadout(board, $"{name}_BufferedReaderAnalog", devices.Where(x => x.ChannelType == PhysicalChannelTypes.AI));
                analogTask.MainTask.Stream.ConfigureInputBuffer(bufferSize);
                analogTask.MainTask.Stream.ReadOverwriteMode = ReadOverwriteMode.OverwriteUnreadSamples;
                analogTask.MainTask.Stream.Timeout = Timeout.Infinite;
                callbackAnalog = new AsyncCallback(ReadCallbackAnalog);
                _daqAnalogReader = new AnalogMultiChannelReader(analogTask.MainTask.Stream);
                _daqAnalogReader.SynchronizeCallbacks = true;
            }

            if (_digitalCounter > 0)
            {
                digitalTask = new DigitalReadout(board, $"{name}_BufferedReaderDigital", devices.Where(x => x.ChannelType == PhysicalChannelTypes.DILine));
                digitalTask.MainTask.Stream.ConfigureInputBuffer(bufferSize);
                digitalTask.MainTask.Stream.ReadOverwriteMode = ReadOverwriteMode.OverwriteUnreadSamples;
                digitalTask.MainTask.Stream.Timeout = Timeout.Infinite;
                callbackDigital = new AsyncCallback(ReadCallbackDigital);
                _daqDigitalReader = new DigitalMultiChannelReader(digitalTask.MainTask.Stream);
                _daqDigitalReader.SynchronizeCallbacks = true;
            }

            (int CounterID, DaqPFI PFI, CounterEdge Edge)[] counterData = counters.ToArray();
            for (int i = 0; i < counterReadouts.Length; i++)
            {
                counterReadouts[i] = new CounterReadout(board, $"{name}_BufferedReaderCounter_{i}", counterData[i].PFI, counterData[i].CounterID, counterData[i].Edge);
                counterReadouts[i].MainTask.Stream.ConfigureInputBuffer(bufferSize);
                counterReadouts[i].MainTask.Stream.ReadOverwriteMode= ReadOverwriteMode.OverwriteUnreadSamples;
                counterReadouts[i].MainTask.Stream.Timeout= Timeout.Infinite;
                callbackCounters[i] = new AsyncCallback(GetCallbackCounter(i));
                _daqCounterReaders[i] = new CounterSingleChannelReader(counterReadouts[i].MainTask.Stream);
                _daqCounterReaders[i].SynchronizeCallbacks = true;
            }
            counterIDCounter = new long[counterReadouts.Length];


            DataBuffer = new DAQSample[bufferSize];
            syncCollection = new SampleSynchronizerCollection(_digitalCounter, _analogCounter, counterReadouts.Length);
            syncCollection.SampleComplete += SyncCollection_SampleComplete;

            this.samplesPerReadCycle = samplesPerReadCycle;
        }

        private void SyncCollection_SampleComplete(object? sender, SampleSynchronizerCollection.SampleEventArgs e)
        {
            LastSample = syncCollection[e.NewSampleID];
        }

        public void ConfigureClock(double rate = 100, DaqPFI? externalClock = null, SampleClockActiveEdge activeEdge = SampleClockActiveEdge.Rising)
        {
            if (analogTask is not null)
            {

                analogTask.MainTask.ExternalClock = externalClock;
                analogTask.MainTask.Timing.ConfigureSampleClock(externalClock == null ? string.Empty : externalClock.ToString(), rate, activeEdge, SampleQuantityMode.ContinuousSamples);
                analogTask.MainTask.Reserve();
                _dataBinAnalog = new double[analogTask.MainTask.AIChannels.Count, samplesPerReadCycle];
            }

            if (digitalTask is not null)
            {
                if (analogTask is not null)
                {
                    digitalTask.MainTask.Timing.ConfigureSampleClock(digitalTask.MainTask.Board.IO.InternalLines[InternalChannelAccess.AI][InternalChannelNames.SampleClock].ToString(),
                                                              rate, activeEdge, SampleQuantityMode.ContinuousSamples);
                }
                else
                {
                    digitalTask.MainTask.Timing.ConfigureSampleClock(externalClock == null ? string.Empty : externalClock.ToString(),
                                                          rate, activeEdge, SampleQuantityMode.ContinuousSamples);
                }

                digitalTask.MainTask.Reserve();
            }

            for (int i = 0; i < counterReadouts.Length; i++)
            {
                if (analogTask is not null)
                {
                    counterReadouts[i].MainTask.Timing.ConfigureSampleClock(counterReadouts[i].MainTask.Board.IO.InternalLines[InternalChannelAccess.AI][InternalChannelNames.SampleClock].ToString(),
                                                              rate, activeEdge, SampleQuantityMode.ContinuousSamples);
                }
                else
                {
                    if (digitalTask is not null)
                    {
                        counterReadouts[i].MainTask.Timing.ConfigureSampleClock(counterReadouts[i].MainTask.Board.IO.InternalLines[InternalChannelAccess.DI][InternalChannelNames.SampleClock].ToString(),
                                                              rate, activeEdge, SampleQuantityMode.ContinuousSamples);
                    }
                    else
                    {
                        counterReadouts[i].MainTask.Timing.ConfigureSampleClock(externalClock == null ? string.Empty : externalClock.ToString(),
                                                          rate, activeEdge, SampleQuantityMode.ContinuousSamples);
                    }
                }

                counterReadouts[i].MainTask.Reserve();
                _dataBinCounter[i] = new int[samplesPerReadCycle];
            }

            syncCollection.SourceRate = rate;
        }

        public void Start()
        {
            analogIDCounter = 0;
            digitalIDCounter = 0;
            
            Array.Fill(counterIDCounter, 0);

            ReadCaret = -1;
            WriteCaret = -1;

            if (digitalTask is not null)
            {
                digitalTask.MainTask.Start();
                _daqDigitalReader!.BeginReadWaveform(samplesPerReadCycle, callbackDigital, digitalTask);
            }

            for (int i = 0; i < counterReadouts.Length; i++)
            {
                counterReadouts[i].MainTask.Start();
                _daqCounterReaders[i].BeginMemoryOptimizedReadMultiSampleInt32(samplesPerReadCycle, callbackCounters[i], counterReadouts[i], _dataBinCounter[i]);
            }

            if (analogTask is not null)
            {
                analogTask.MainTask.Start();
                _daqAnalogReader!.BeginMemoryOptimizedReadMultiSample(samplesPerReadCycle, callbackAnalog, analogTask, _dataBinAnalog, ReallocationPolicy.DoNotReallocate);
            }

            
        }

        public void Stop()
        {
            analogTask?.MainTask?.Abort();
            digitalTask?.MainTask.Abort();
            foreach(var task in counterReadouts)
            {
                task.MainTask?.Abort();
            }
        }

        public DAQSample[] ReadAll()
        {
            if (SamplesBehind < 0)
                throw new BufferOverflowException(this);


            DAQSample[] data = new DAQSample[SamplesBehind];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = Read()!;
            }

            return data;
        }

        public DAQSample? Read()
        {
            lock (readLock)
            {
                if (SamplesBehind > 0)
                {
                    ReadCaret++;
                    var data = DataBuffer[ReadCaret % DataBuffer.Length];
                    return data;
                }
                return null;
            }
            
            
        }

        Action<IAsyncResult> GetCallbackCounter(int counterInternalID)
        {
            return new Action<IAsyncResult>((ar) => ReadCallbackCounter(ar, counterInternalID));
        }

        private void ReadCallbackCounter(IAsyncResult ar, int counterInternalID)
        {
            int[] dataOut;
            int samplesRead;
            try
            {
                dataOut = _daqCounterReaders[counterInternalID].EndMemoryOptimizedReadMultiSampleInt32(ar, out samplesRead);
            }
            catch { return; }
            if (counterReadouts[counterInternalID].MainTask.IsRunning)
                _daqCounterReaders[counterInternalID].BeginMemoryOptimizedReadMultiSampleInt32(samplesPerReadCycle, callbackCounters[counterInternalID], counterReadouts[counterInternalID], _dataBinCounter[counterInternalID]);

            for (int i = 0; i < samplesRead; i++)
            {
                syncCollection.AddCounterSample(dataOut[i]-1,counterInternalID, counterIDCounter[counterInternalID]++);
            }

        }

        private void ReadCallbackAnalog(IAsyncResult ar)
        {
            double[,] dataOut;
            int samplesRead;
            try
            {
                dataOut = _daqAnalogReader!.EndMemoryOptimizedReadMultiSample(ar, out samplesRead);
            } catch { return; }
            if (analogTask!.MainTask.IsRunning)
                _daqAnalogReader.BeginMemoryOptimizedReadMultiSample(samplesPerReadCycle, callbackAnalog, analogTask, _dataBinAnalog);


            for (int i = 0; i < samplesRead; i++)
            {


                double[] _ls = new double[dataOut.GetUpperBound(0) + 1];

                for (int j = 0; j < _ls.Length; j++)
                {
                    _ls[j] = dataOut[j, i];
                }

                syncCollection.AddAnalogSample(_ls, analogIDCounter++);
            }

        }

        private void ReadCallbackDigital(IAsyncResult ar)
        {
            NationalInstruments.DigitalWaveform[] dataOut;
            int samplesRead;
            try
            {
                dataOut = _daqDigitalReader!.EndReadWaveform(ar);
                samplesRead = dataOut[0].Signals.Count;
            }
            catch (Exception ex)
            {
                return;
            }



            if (digitalTask!.MainTask.IsRunning)
                _daqDigitalReader!.BeginReadWaveform(samplesPerReadCycle, callbackDigital, digitalTask);

            for (int i = 0; i < samplesRead; i++)
            {

                bool[] _ls = new bool[dataOut.Length];

                for (int j = 0; j < dataOut.Length; i++)
                {
                    _ls[j] = dataOut[j].Samples[i].States[0] == NationalInstruments.DigitalState.ForceUp;
                }

                syncCollection.AddDigitalSample(_ls, digitalIDCounter++);
            }
        }

        public void Dispose()
        {
            analogTask?.Dispose();
            digitalTask?.Dispose();
            foreach(var task in counterReadouts)
            {
                task.Dispose();
            }
        }
    }
}
