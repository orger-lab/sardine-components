using NationalInstruments.DAQmx;
using Sardine.Utils.Measurements.Time;
using Sardine.Utils.Measurements;

namespace Sardine.Devices.NI.DAQ.Controllers
{
    public class ClockController(DAQBoard board, string name, int counterOutputID, Measure<Hertz> samplingRate, DaqPFI clockOutput,
                                  int pointsPerClockIn = 1, DaqPFI? clockSource = null) : DAQDevice(board, name, (samplingRate, pointsPerClockIn, counterOutputID, clockSource))
    {
        public Measure<Hertz> SamplingRate { get; } = samplingRate;
        public DaqPFI ClockOutput { get; } = clockOutput;
        public override void SetupChannels()
        {
            var parameters = ((Measure<Hertz> RateUnit, int PointsPerClockIn, int CounterID, DaqPFI? ClockSource))ChannelParameters!;

            MainTask.COChannels.CreatePulseChannelFrequency(MainTask.Board.IO.CounterOut[parameters.CounterID].ToString(), $"{MainTask.Name}_ClockCounter_Out", COPulseFrequencyUnits.Hertz, COPulseIdleState.Low, 0, parameters.RateUnit, 0.5);
            if (parameters.ClockSource is not null)
            {
                MainTask.Timing.ConfigureImplicit(SampleQuantityMode.FiniteSamples, parameters.PointsPerClockIn);
                MainTask.Triggers.StartTrigger.ConfigureDigitalEdgeTrigger(parameters.ClockSource.ToString(), DigitalEdgeStartTriggerEdge.Falling);
                MainTask.Triggers.StartTrigger.Retriggerable = true;
                return;
            }

            MainTask.Timing.ConfigureImplicit(SampleQuantityMode.ContinuousSamples);
            MainTask.Triggers.StartTrigger.ConfigureNone();
        }
    }
}
