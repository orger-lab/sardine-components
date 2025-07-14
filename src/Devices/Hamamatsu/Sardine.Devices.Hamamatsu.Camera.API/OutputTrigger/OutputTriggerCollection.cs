using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    public class OutputTriggerCollection : IEnumerable<OutputTrigger>, INotifyCollectionChanged
    {
        private readonly DCam dCam;
        private readonly ObservableCollection<OutputTrigger> collection;

        public int Count { get; }

        public OutputTrigger this[int i]
        {
            get
            {
                return collection[i];
            }
            set
            {
                if (!collection[i].Equals(value))
                {
                    value.Set(dCam, i + 1);
                    collection[i] = OutputTrigger.Get(dCam, i + 1);
                }
            }
        }

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public OutputTriggerCollection(DCam dcam, OutputTrigger[] triggers)
        {
            dCam = dcam;
            collection = new ObservableCollection<OutputTrigger>(triggers);
            collection.CollectionChanged += (s, e) => CollectionChanged?.Invoke(this, e);
            Count = triggers.Length;
        }

        public IEnumerator<OutputTrigger> GetEnumerator() => ((IEnumerable<OutputTrigger>)collection).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)collection).GetEnumerator();
    }
}
