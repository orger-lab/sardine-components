using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Diagnostics;
using System.Threading;

[Combinator]
[Description("")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class BusyWorkerScript
{
    public IObservable<int> Process(IObservable<int> source)
    {
        return source.Select(value => WorkThreads());
    }

       private uint numThreads = 12;
       private uint busyDifficulty = 1;
       private ulong busyCycles = 10000;

       public ulong BusyCycles
       {
           get
           {
               return busyCycles;
           }
           set
           {
               busyCycles = Math.Max(1, value);
           }
       }
       public uint BusyDifficulty
       {
           get
           {
                return busyDifficulty;
           }
           set
           {
               busyDifficulty = Math.Max(1, Math.Min(20, value));
           }
       }
       public uint NumThreads
       {
           get
           {
               return numThreads;
           }
           set
           {
               numThreads = value;
           }
       }

       public Random RandEngine { get; set; }

       public BusyWorkerScript()
       {
           RandEngine = new Random(DateTime.Now.GetHashCode());
       }


       public int WorkThreads()
       {
           CountdownEvent countdown = new CountdownEvent((int)NumThreads);
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
           
           // Console.WriteLine(valueFinal);
           return (int)Math.Ceiling(Math.Log10(valueFinal));
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

           return ComputeSmallFactorial(value - 1) * (ulong)Math.Ceiling((double)value*(1-RandEngine.NextDouble()*0.05));
       }

}
