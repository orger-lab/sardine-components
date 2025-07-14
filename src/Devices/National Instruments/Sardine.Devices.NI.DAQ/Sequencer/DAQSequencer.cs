using NationalInstruments.DAQmx;
using Sardine.Constructs;
using Sardine.Devices.NI.DAQ.Controllers;
using Sardine.Sequencer;
using Sardine.Utils.Measurements;
using Sardine.Utils.Measurements.Electric;
using Sardine.Utils.Measurements.Time;
using Sardine.Utils.Waveforms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sardine.Devices.NI.DAQ.Sequencer
{
    public class DAQSequencer : ISequencer, IRangeProvider<Volt>, IDisposable
    {
        private MultiDigitalOutputDAQTask? digitalTask;
        private MultiAnalogOutputDAQTask? analogTask;
        
        public bool IsArming { get; private set; }
        public Measure<Volt> MinValue => -10;
        public Measure<Volt> MaxValue => 10;

        public IReadOnlyList<DaqPhysicalChannelID> AnalogOutChannels { get; }
        public IReadOnlyList<DaqPhysicalChannelID> DigitalOutChannels { get; }
        public IReadOnlyList<IVirtualSequencerOutputChannel> VirtualOutChannels { get; }
        public IReadOnlyList<DaqPhysicalChannelID> AnalogInChannels { get; }
        public IReadOnlyList<(int, DaqPFI, CounterEdge)> CounterInChannels { get; }

        public ClockSource ClockGenerator { get; private set; }
        public bool IsRunning { get; private set; }

        public bool IsArmed { get; private set; }

        public DAQBoard Board { get; }
        public string Name { get; }

        DAQTask? mainTask;
        private int currentPattern;


        public Measure<Second> ExpectedTimeLeft => ExecutionRate == 0 ? 0 : Math.Max(0, (TotalSequenceLength - CurrentPattern) / ExecutionRate);

        public Measure<Hertz> ExecutionRate { get; private set; } = 0;
        public double ExecutionProgressPercentage => TotalSequenceLength == 0 ? 0 : Math.Min(100, 100 * CurrentPattern / TotalSequenceLength);

        public double DefaultAnalogValue { get; }
        public bool DefaultDigitalValue { get; }

        BufferedDAQChannelReader? channelReader;

        public static DAQSample? Source(DAQSequencer sequencer, out bool hasMore)
        {
            hasMore = sequencer.channelReader?.SamplesBehind > 1;
            return sequencer.channelReader?.Read();
        }
        public bool HasData => channelReader?.SamplesBehind > 0;

        public event EventHandler<OnCaptureFinishedEventArgs>? OnCaptureFinished;

        public delegate void PreCapture(bool isRecording);
        public PreCapture PreCaptureAction { get; set; } = x => { return; };

        public BlockDescriptionCollection BlockDescriptions { get; } = new BlockDescriptionCollection();

        public class BlockDescriptionCollection()
        {
            private List<string> header = [];
            private Dictionary<string, IReadOnlyList<IReadOnlyList<SequencerOutputValue>>> blockDescriptions = [];

            public IReadOnlyList<string> Header => header; public IReadOnlyDictionary<string, IReadOnlyList<IReadOnlyList<SequencerOutputValue>>> BlockDescriptions => blockDescriptions;
            internal void ClearBlockDescriptions()
            {
                blockDescriptions = new Dictionary<string, IReadOnlyList<IReadOnlyList<SequencerOutputValue>>>();
                header = [];
            }

            internal void AddBlockDescription(string name, IReadOnlyList<IReadOnlyList<SequencerOutputValue>> description)
            {
                blockDescriptions[name] = description;
            }

            internal void AddHeader(List<string> header)
            {
                this.header = header;
            }
        }




        public DAQSequencer(DAQBoard board, string name, ClockSource clockGenerator,
                            IList<DaqPhysicalChannelID>? analogOuts = null,
                            IList<DaqPhysicalChannelID>? digitalOuts = null,
                            IList<IVirtualSequencerOutputChannel>? virtualOuts = null,
                            IList<DaqPhysicalChannelID>? analogIns = null,
                            IList<(int, DaqPFI, CounterEdge)>? counterIns = null)
        {
            digitalOuts ??= [];
            analogOuts ??= [];
            virtualOuts ??= [];
            analogIns ??= [];
            counterIns ??= [];

            if (digitalOuts.Any(x => x.ChannelType != PhysicalChannelTypes.DOLine))
                throw new ArgumentOutOfRangeException(nameof(digitalOuts));

            if (analogOuts.Any(x => x.ChannelType != PhysicalChannelTypes.AO))
                throw new ArgumentOutOfRangeException(nameof(analogOuts));

            DigitalOutChannels = (IReadOnlyList<DaqPhysicalChannelID>)digitalOuts;
            AnalogOutChannels = (IReadOnlyList<DaqPhysicalChannelID>)analogOuts;
            VirtualOutChannels = (IReadOnlyList<IVirtualSequencerOutputChannel>)virtualOuts;
            AnalogInChannels = (IReadOnlyList<DaqPhysicalChannelID>)analogIns;
            CounterInChannels = (IReadOnlyList<(int, DaqPFI, CounterEdge)>)counterIns;

            ClockGenerator = clockGenerator;

            Board = board;
            Name = name;

            TotalChannelCount = AnalogOutChannels.Count + DigitalOutChannels.Count + VirtualOutChannels.Count;
            virtualOutputMap = [];
        }

        public int SequenceLength { get; private set; }
        public bool CollectReadouts { get; set; } = false;

        public void Arm(Measure<Hertz> sampleRate, int patternLength, IList<(int, IPatternBlock)> blockSequence)
        {
            IsArming = true;
            BlockDescriptions.ClearBlockDescriptions();

            if (mainTask is not null)
            {
                mainTask.OnTaskStateChanged -= MainTask_OnTaskStateChanged;
                mainTask.EveryNSamplesWritten -= MainTask_EveryNSamplesWritten;
                mainTask.Dispose();
                mainTask = null;
            }
            Dictionary<DaqPhysicalChannelID, int> usableAnalogs = [];
            Dictionary<DaqPhysicalChannelID, int> usableDigitals = [];


            foreach (var blockInfo in blockSequence)
            {
                foreach (var key in blockInfo.Item2.AnalogPatterns)
                {
                    usableAnalogs.TryAdd((DaqPhysicalChannelID)key.Key, AnalogOutChannels.ToList().IndexOf((DaqPhysicalChannelID)key.Key));
                }
                foreach (var key in blockInfo.Item2.DigitalPatterns)
                {
                    usableDigitals.TryAdd((DaqPhysicalChannelID)key.Key, DigitalOutChannels.ToList().IndexOf((DaqPhysicalChannelID)key.Key));
                }
            }


            BlockDescriptions.AddHeader(usableAnalogs.Keys.Select(x => x.FriendlyName).Concat(usableDigitals.Keys.Select(x => x.FriendlyName)).Concat(VirtualOutChannels.Select(x => x.Name)).ToList());

            ClockGenerator.Rate = sampleRate;

            CurrentPattern = -1;
            ExecutionRate = ClockGenerator.Rate / patternLength;

            SequenceLength = blockSequence.Select(x => x.Item1 * x.Item2.NumPatterns).Sum();

            double[,] analogOutBuilder = new double[usableAnalogs.Count, SequenceLength * patternLength];
            NationalInstruments.DigitalWaveform[] digitalOutBuilder = new NationalInstruments.DigitalWaveform[usableDigitals.Count];
            virtualOutputMap = [];

            for (int i = 0; i < usableDigitals.Count; i++)
            {
                digitalOutBuilder[i] = new NationalInstruments.DigitalWaveform(SequenceLength * patternLength, 1);
            }

            int patternCaret = 0;

            foreach ((int NumReps, DAQPatternBlock Block) blockEntry in blockSequence)
            {
                double[,] analogSequence = new double[usableAnalogs.Count, patternLength * blockEntry.Block.NumPatterns];
                bool[,] digitalSequence = new bool[usableDigitals.Count, patternLength * blockEntry.Block.NumPatterns];
                object?[,] virtualSequence = new string[VirtualOutChannels.Count, blockEntry.Block.NumPatterns];

                for (int columnID = 0; columnID < patternLength * blockEntry.Block.NumPatterns; columnID++)
                {

                    for (int rowID = 0; rowID < usableAnalogs.Count; rowID++)
                    {
                        analogSequence[rowID, columnID] = DefaultAnalogValue;
                    }

                    for (int rowID = 0; rowID < usableDigitals.Count; rowID++)
                    {
                        digitalSequence[rowID, columnID] = DefaultDigitalValue;
                    }

                    if (columnID < blockEntry.Block.NumPatterns)
                    {
                        for (int rowID = 0; rowID < VirtualOutChannels.Count; rowID++)
                        {
                            virtualSequence[rowID, columnID] = null;
                        }
                    }
                }

                for (int rowID = 0; rowID < usableAnalogs.Count; rowID++)
                {
                    blockEntry.Block.AnalogPatterns.TryGetValue(usableAnalogs.Keys.ElementAt(rowID), out IList<IAnalogOutputProvider?>? providerList);
                    providerList ??= [];
                    IAnalogOutputProvider? lastProvider = null;
                    List<IAnalogOutputProvider> knownProviders = [];

                    for (int columnID = 0; columnID < blockEntry.Block.NumPatterns; columnID++)
                    {
                        IAnalogOutputProvider? analogProvider = providerList.ElementAtOrDefault(columnID) ?? (blockEntry.Block.FillEmptyWithLast ? lastProvider : null);
                        if (analogProvider is not null)
                        {
                            if (!knownProviders.Contains(analogProvider))
                            {
                                knownProviders.Add(analogProvider);
                                analogProvider.Reset();
                            }
                            double[] waveform = analogProvider.GenerateWaveform(patternLength, ClockGenerator.Rate);
                            for (int waveformID = 0; waveformID < Math.Min(waveform.Length, patternLength * (blockEntry.Block.NumPatterns - columnID)); waveformID++)
                            {
                                analogSequence[rowID, columnID * patternLength + waveformID] = waveform[waveformID];
                            }
                            lastProvider = analogProvider;
                        }
                    }
                }

                for (int rowID = 0; rowID < usableDigitals.Count; rowID++)
                {
                    blockEntry.Block.DigitalPatterns.TryGetValue(usableDigitals.Keys.ElementAt(rowID), out IList<IDigitalOutputProvider?>? providerList);
                    providerList ??= [];
                    IDigitalOutputProvider? lastProvider = null;
                    List<IDigitalOutputProvider> knownProviders = [];

                    for (int columnID = 0; columnID < blockEntry.Block.NumPatterns; columnID++)
                    {
                        IDigitalOutputProvider? digitalProvider = providerList.ElementAtOrDefault(columnID) ?? (blockEntry.Block.FillEmptyWithLast ? lastProvider : null);
                        if (digitalProvider is not null)
                        {
                            if (!knownProviders.Contains(digitalProvider))
                            {
                                knownProviders.Add(digitalProvider);
                                digitalProvider.Reset();
                            }
                            bool[] waveform = digitalProvider.GenerateTriggers(patternLength, ClockGenerator.Rate);
                            for (int waveformID = 0; waveformID < Math.Min(waveform.Length, patternLength * (blockEntry.Block.NumPatterns - columnID)); waveformID++)
                            {
                                digitalSequence[rowID, columnID * patternLength + waveformID] = waveform[waveformID];
                            }
                        }
                    }
                }

                for (int rowID = 0; rowID < VirtualOutChannels.Count; rowID++)
                {
                    blockEntry.Block.VirtualPatterns.TryGetValue(VirtualOutChannels[rowID], out IList<object?>? providerList);
                    providerList ??= [];
                    object? lastProvider = null;

                    for (int columnID = 0; columnID < blockEntry.Block.NumPatterns; columnID++)
                    {
                        object? virtualProvider = providerList.ElementAtOrDefault(columnID) ?? (blockEntry.Block.FillEmptyWithLast ? lastProvider : null);
                        if (virtualProvider is not null)
                        {
                            virtualSequence[rowID, columnID] = virtualProvider;
                        }
                    }
                }



                if (!BlockDescriptions.BlockDescriptions.ContainsKey(blockEntry.Block.BlockName))
                {
                    List<List<SequencerOutputValue>> outputData = [];

                    for (int i = 0; i < patternLength * blockEntry.Block.NumPatterns; i++)
                    {
                        List<SequencerOutputValue> rowData = [];

                        for (int j = 0; j < usableAnalogs.Count; j++)
                        {
                            rowData.Add(new SequencerOutputValue(analogSequence[j, i]));
                        }
                        for (int j = 0; j < usableDigitals.Count; j++)
                        {
                            rowData.Add(new SequencerOutputValue(digitalSequence[j, i]));
                        }
                        if (i % patternLength == 0)
                        {
                            for (int j = 0; j < VirtualOutChannels.Count; j++)
                            {
                                rowData.Add(new SequencerOutputValue(virtualSequence[j, i / patternLength]?.ToString()));
                            }
                        }

                        outputData.Add(rowData);
                    }


                    BlockDescriptions.AddBlockDescription(blockEntry.Block.BlockName, outputData);
                }






                for (int reps = 0; reps < blockEntry.NumReps; reps++)
                {
                    for (int analogChannelID = 0; analogChannelID < usableAnalogs.Count; analogChannelID++)
                    {
                        for (int sample = 0; sample < patternLength * blockEntry.Block.NumPatterns; sample++)
                        {
                            analogOutBuilder[analogChannelID, sample + patternCaret * patternLength] = analogSequence[analogChannelID, sample];
                        }

                    }
                    for (int digitalChannelID = 0; digitalChannelID < usableDigitals.Count; digitalChannelID++)
                    {
                        for (int sample = 0; sample < patternLength * blockEntry.Block.NumPatterns; sample++)
                        {
                            digitalOutBuilder[digitalChannelID].Samples[sample + patternCaret * patternLength].States[0] = digitalSequence[digitalChannelID, sample] ? NationalInstruments.DigitalState.ForceUp : NationalInstruments.DigitalState.ForceDown;
                        }
                    }

                    for (int j = 0; j < VirtualOutChannels.Count; j++)
                    {
                        for (int k = 0; k < blockEntry.Block.NumPatterns; k++)
                        {
                            if (virtualSequence[j, k] is null)
                                continue;

                            virtualOutputMap.TryAdd(patternCaret + k, []);
                            virtualOutputMap[patternCaret + k].Add((VirtualOutChannels[j], virtualSequence[j, k]!));
                        }
                    }

                    patternCaret += blockEntry.Block.NumPatterns;
                }
            }

            if (usableDigitals.Count > 0)
            {
                digitalTask = new MultiDigitalOutputDAQTask(Board, $"DigitalSequencerOut_{Name}", usableDigitals.Keys);
                digitalTask.MainTask.ExternalClock = ClockGenerator.ClockOutput;
                digitalTask.MainTask.ConfigureClock(ClockGenerator.Rate, SequenceLength * patternLength, repeatSamples: true);
                digitalTask.MainTask.Reserve();
                digitalTask.DigitalChannelWriter.WriteWaveform(false, digitalOutBuilder);
            }

            if (usableAnalogs.Count > 0)
            {
                analogTask = new MultiAnalogOutputDAQTask(Board, $"AnalogSequencerOut_{Name}", usableAnalogs.Keys, MinValue, MaxValue);
                analogTask.MainTask.ExternalClock = ClockGenerator.ClockOutput;
                analogTask.MainTask.ConfigureClock(ClockGenerator.Rate, SequenceLength * patternLength, repeatSamples: true);
                analogTask.MainTask.Reserve();
                analogTask.AnalogChannelWriter.WriteMultiSample(false, analogOutBuilder);
            }

            if (analogTask is null)
            {
                if (digitalTask is null)
                {
                    IsArming = false;
                    return;
                }

                mainTask = digitalTask.MainTask;
            }
            else
            {
                mainTask = analogTask.MainTask;
            }

            mainTask.EveryNSamplesWrittenEventInterval = patternLength;
            mainTask.OnTaskStateChanged += MainTask_OnTaskStateChanged;
            mainTask.EveryNSamplesWritten += MainTask_EveryNSamplesWritten;

            ClockGenerator.Arm();

            if (CollectReadouts && AnalogInChannels.Count > 0)
            {
                channelReader = new BufferedDAQChannelReader(Board, "Sequencer_Readout", AnalogInChannels, CounterInChannels, 1000 * patternLength, patternLength);
                channelReader.NewSampleAvailable += (_, _) => OnReaderData?.Invoke(this, EventArgs.Empty);
                channelReader.ConfigureClock(ClockGenerator.Rate, ClockGenerator.ClockOutput);
            }

            IsArmed = true;
            IsArming = false;
        }

        public event EventHandler? OnReaderData;

        public void Disarm()
        {
            IsArming = true;
            ClockGenerator.Disarm();
            analogTask?.MainTask.Unreserve();
            digitalTask?.MainTask.Unreserve();
            IsArmed = false;
            IsArming = false;
        }

        Dictionary<int, List<(IVirtualSequencerOutputChannel, object)>> virtualOutputMap;


        public event EventHandler? OnPatternChanged;
        public event EventHandler? OnRunningStateChanged;

        public int CurrentPattern
        {
            get => currentPattern;
            private set
            {
                currentPattern = value;
                if (IsRunning)
                {

                    if (virtualOutputMap.TryGetValue(value % SequenceLength, out List<(IVirtualSequencerOutputChannel, object)>? activeSequencerList))
                    {
                        foreach ((IVirtualSequencerOutputChannel ActiveChannel, object ActivationObject) activationElement in activeSequencerList)
                        {
                            new System.Threading.Tasks.Task(() => activationElement.ActiveChannel.Actuate(activationElement.ActivationObject)).Start();
                        }
                    }
                    if (value > TotalSequenceLength && TotalSequenceLength > 0)
                    {
                        Stop(forced: false);
                    }
                }

                OnPatternChanged?.Invoke(this, EventArgs.Empty);
            }
        }


        private void MainTask_EveryNSamplesWritten(object sender, EveryNSamplesWrittenEventArgs e)
        {
            if (IsRunning)
            {
                CurrentPattern++;
            }
        }

        private void MainTask_OnTaskStateChanged(object? sender, NITaskStateEventArgs e)
        {
            if (e.NewState == TaskState.Running)
                IsRunning = true;
            else
            {
                ClockGenerator.Stop();
                ClockGenerator.Disarm();
                IsRunning = false;
            }

            OnRunningStateChanged?.Invoke(this, e);
        }

        public int TotalSequenceLength { get; private set; } = 0;
        public int TotalChannelCount { get; }


        public void Start(int stopAfterNPatterns = 0)
        {
            if (IsRunning)
                return;

            TotalSequenceLength = stopAfterNPatterns;

            analogTask?.MainTask.Start();
            digitalTask?.MainTask.Start();
            channelReader?.Start();
            ClockGenerator.Start();
            CurrentPattern++;
        }

        public void Stop(bool forced = true)
        {
            if (!IsRunning)
                return;

            ClockGenerator.Stop();

            if (analogTask?.MainTask.IsRunning ?? false)
                analogTask?.MainTask.Stop();

            if (digitalTask?.MainTask.IsRunning ?? false)
                digitalTask?.MainTask.Stop();


            analogTask?.Dispose();
            analogTask = null;

            digitalTask?.Dispose();
            digitalTask = null;

            channelReader?.Stop();
            channelReader?.Dispose();
            channelReader = null;


            Disarm();

            OnCaptureFinished?.Invoke(this, new OnCaptureFinishedEventArgs(forced));
        }

        public void Dispose()
        {
            mainTask?.Dispose();
            analogTask?.Dispose();
            digitalTask?.Dispose();
            channelReader?.Dispose();
        }
    }

}
