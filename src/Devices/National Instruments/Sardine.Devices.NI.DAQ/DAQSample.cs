using Sardine.Recording.Data.Text;

using System;
using System.Linq;

namespace Sardine.Devices.NI.DAQ
{
    public class DAQSample : ITextWritable, ICloneable
    {
        public long ID { get; internal set; }
        public double[] Analog { get; set; }
        public int?[] Counter { get; set; }
        public bool[] Digital { get; set; }

        public double Rate { get; }
        public bool IsAlive => true;

        internal int FillCount => Analog.Length + Digital.Length + Counter.Where(x => x is not null).Count();

        public DAQSample(long id, double rate = 1)
        {
            ID = id;
            Digital = [];
            Analog = [];
            Counter = [];
            Rate = rate;
        }

        public string WriteHeader()
        {
            string header = $"ID_{Rate}Hz ";

            for (int i = 0; i < Analog.Length; i++)
            {
                header += $"Analog{i} ";
            }

            for (int i = 0; i < Digital.Length; i++)
            {
                header += $"Digital{i} ";
            }

            for (int i = 0; i < Counter.Length; i++)
            {
                header += $"Counter{i} ";
            }

            return header;
        }

        public string WriteData()
        {
            string data = $"{ID} ";

            for (int i = 0; i < Analog.Length; i++)
            {
                data += $"{Analog[i]} ";
            }

            for (int i = 0; i < Digital.Length; i++)
            {
                data += $"{Digital[i]} ";
            }

            for (int i = 0; i < Counter.Length; i++)
            {
                data += $"{Counter[i]} ";
            }

            return data;
        }

        public object Clone()
        {
            return new DAQSample(ID, Rate) {  Analog= (double[])Analog.Clone(), Digital = (bool[])Digital.Clone(), Counter = (int?[])Counter.Clone() };
        }
    }
}
