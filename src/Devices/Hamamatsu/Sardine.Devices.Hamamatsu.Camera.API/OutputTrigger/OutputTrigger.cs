using System;

namespace Sardine.Devices.Hamamatsu.Camera.API
{
    public class OutputTriggerWithID
    {
        public OutputTrigger Trigger {get;}
        public int ID { get; set; }

        public static implicit operator OutputTrigger(OutputTriggerWithID otID)
        {
            return otID.Trigger;
        }

        public OutputTriggerWithID(OutputTrigger trigger, int iD)
        
        {
            Trigger = trigger;
            ID = iD;
        }
    }

    public class OutputTrigger : IEquatable<OutputTrigger>
    {
        public OutputTriggerSource Source { get; internal set; }
        public OutputTriggerPolarity Polarity { get; internal set; }
        public OutputTriggerActiveRegion ActiveRegion { get; internal set; }
        public OutputTriggerKind Kind { get; internal set; }
        public double Delay { get; internal set; }
        public double Period { get; internal set; }

        internal OutputTrigger() { }

        public bool Equals(OutputTrigger? other)
        {
            if (other is null) return false;

            return
                    Source == other.Source
                 && Polarity == other.Polarity
                 && ActiveRegion == other.ActiveRegion
                 && Kind == other.Kind
                 && Delay == other.Delay
                 && Period == other.Period
                 ;

        }
        public void Set(DCam dCam, int channelID) //channelID starts at 1
        {
            if (dCam.PropertyCollection.NumberOfOutputTriggerConnector is null) throw new InvalidOperationException();
            if (dCam.PropertyCollection.NumberOfOutputTriggerConnector.Value < channelID) throw new InvalidOperationException();
            if (dCam.PropertyCollection.OutputTriggerActiveRegion is null) throw new InvalidOperationException();
            if (dCam.PropertyCollection.OutputTriggerKind is null) throw new InvalidOperationException();
            if (dCam.PropertyCollection.OutputTriggerPeriod is null) throw new InvalidOperationException();
            if (dCam.PropertyCollection.OutputTriggerPolarity is null) throw new InvalidOperationException();
            if (dCam.PropertyCollection.OutputTriggerSource is null) throw new InvalidOperationException();
            if (dCam.PropertyCollection.OutputTriggerDelay is null) throw new InvalidOperationException();

            dCam.PropertyCollection.OutputTriggerActiveRegion[channelID - 1] = ActiveRegion;
            dCam.PropertyCollection.OutputTriggerKind[channelID - 1] = Kind;
            dCam.PropertyCollection.OutputTriggerPeriod[channelID - 1] = Period;
            dCam.PropertyCollection.OutputTriggerPolarity[channelID - 1] = Polarity;
            try
            {
                dCam.PropertyCollection.OutputTriggerSource[0] = Source; //FUN THINGS HERE SOURCE IS APPARENTLY THE SAME FOR EVERY CHANNEL? CHECK CAREFULLY USE CHANNEL 1 JUST IN CASE
            }
            catch { }
        }

        public static OutputTrigger Get(DCam dCam, int channelID) // channelID starts at 1
        {
            if (dCam.PropertyCollection.NumberOfOutputTriggerConnector is null) throw new InvalidOperationException();
            if (dCam.PropertyCollection.NumberOfOutputTriggerConnector.Value < channelID) throw new InvalidOperationException();
            if (dCam.PropertyCollection.OutputTriggerActiveRegion is null) throw new InvalidOperationException();
            if (dCam.PropertyCollection.OutputTriggerKind is null) throw new InvalidOperationException();
            if (dCam.PropertyCollection.OutputTriggerPeriod is null) throw new InvalidOperationException();
            if (dCam.PropertyCollection.OutputTriggerPolarity is null) throw new InvalidOperationException();
            if (dCam.PropertyCollection.OutputTriggerSource is null) throw new InvalidOperationException();
            if (dCam.PropertyCollection.OutputTriggerDelay is null) throw new InvalidOperationException();

            return new OutputTrigger()
            {
                ActiveRegion = dCam.PropertyCollection.OutputTriggerActiveRegion[channelID - 1],
                Kind = dCam.PropertyCollection.OutputTriggerKind[channelID - 1],
                Period = dCam.PropertyCollection.OutputTriggerPeriod[channelID - 1],
                Polarity = dCam.PropertyCollection.OutputTriggerPolarity[channelID - 1],
                Source = dCam.PropertyCollection.OutputTriggerSource[0],
                Delay = dCam.PropertyCollection.OutputTriggerDelay[channelID - 1],
            };
        }

        public override bool Equals(object? obj) => obj is OutputTrigger objS && Equals(objS);
        public override int GetHashCode() => (Source, Polarity, ActiveRegion, Kind, Delay,Period).GetHashCode();
    }
}
