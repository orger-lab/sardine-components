using System;
using System.Collections.Generic;

namespace Sardine.Devices.NI.DAQ.Controllers
{
    public partial class BufferedDAQChannelReader
    {
        internal class SampleSynchronizerCollection
        {
            private readonly Dictionary<long, DAQSample> _storage;

            public event EventHandler<SampleEventArgs>? SampleComplete;

            readonly object _lock;

            public double SourceRate { get; set; } = 0;
            public int NDigital { get; }
            public int NAnalog { get; }
            public int NCounter { get; }

            public int NEntries => NDigital + NAnalog + NCounter;

            public SampleSynchronizerCollection(int nDigital = 0, int nAnalog = 0, int nCounter = 0)
            {
                _storage = new Dictionary<long, DAQSample>();
                _lock = new object();
                NDigital = nDigital;
                NAnalog = nAnalog;
                NCounter = nCounter;
            }

            public DAQSample this[long index]
            {
                get
                {
                    DAQSample returnValue = (DAQSample)_storage[index].Clone();
                    _storage.Remove(index);
                    return returnValue;
                }
            }

            public void AddCounterSample(int sample, int sampleID, long id)
            {
                if (!_storage.ContainsKey(id))
                {
                    _storage.Add(id, new DAQSample(id, SourceRate));
                    if (_storage[id].Counter.Length == 0 && NCounter > 0)
                    {
                        _storage[id].Counter = new int?[NCounter];
                        Array.Fill<int?>(_storage[id].Counter, null);
                    }

                }

                _storage[id].Counter[sampleID] = sample;

                if (_storage[id].FillCount == NEntries)
                    SampleComplete?.Invoke(this, new SampleEventArgs(id));
            }

            public void AddAnalogSample(double[] sample, long id)
            {
                    if (_storage.ContainsKey(id))
                    {
                        _storage[id].Analog = sample;
                    }
                    else
                    {
                        _storage.Add(id, new DAQSample(id, SourceRate) { Analog = sample });
                    if (_storage[id].Counter.Length == 0 && NCounter > 0)
                    {
                        _storage[id].Counter = new int?[NCounter];
                        Array.Fill<int?>(_storage[id].Counter, null);
                    }
                }
                if (_storage[id].FillCount == NEntries)
                    SampleComplete?.Invoke(this, new SampleEventArgs(id));
            }

            public void AddDigitalSample(bool[] sample, long id)
            {
                    if (_storage.ContainsKey(id))
                    {
                        _storage[id].Digital = sample;
                    }
                    else
                    {
                        _storage.Add(id, new DAQSample(id, SourceRate) { Digital = sample});
                    if (_storage[id].Counter.Length == 0 && NCounter > 0)
                    {
                        _storage[id].Counter = new int?[NCounter];
                        Array.Fill<int?>(_storage[id].Counter, null);
                    }
                }
                if (_storage[id].FillCount == NEntries)
                    SampleComplete?.Invoke(this, new SampleEventArgs(id));

            }

            public class SampleEventArgs : EventArgs
            {
                public long NewSampleID { get; private set; }
                public SampleEventArgs(long id) : base()
                {
                    NewSampleID = id;
                }
            }

        }
    }


}
