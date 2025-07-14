using Sardine.Utils.Measurements.Time;
using Sardine.Utils.Measurements;

namespace Sardine.Devices.NI.DAQ.Controllers
{
    public sealed class ClockSource(DAQBoard board, string name, int counterOutputID, DaqPFI clockOutput)
    {
        public DAQBoard Board { get; } = board;
        public string Name { get; } = name;
        public int CounterOutputID { get; } = counterOutputID;
        public DaqPFI ClockOutput { get; } = clockOutput;
        public bool ClockRunning { get; private set; }

        public bool ClockArmed { get; private set; }

        public Measure<Hertz> Rate { get; set; } = 1;

        ClockController? controller;
        public ClockController Controller => controller ?? GetNewClock();

        ClockController GetNewClock()
        {
            if (ClockArmed)
                throw new System.Exception("clock armed.");

            if (controller is not null)
            {
                controller.OnTaskStateChanged -= Controller_OnTaskStateChanged;
                controller.Dispose();
                controller = null;
            }
            controller = new ClockController(Board, Name, CounterOutputID, Rate, ClockOutput);
            controller.OnTaskStateChanged += Controller_OnTaskStateChanged;

            return controller;
        }

        public ClockController Arm()
        {
            if (ClockRunning)
                throw new System.Exception("clock running");

            GetNewClock();
            ClockArmed = true;
            return Controller;
        }

        public void Disarm()
        {
            if (ClockRunning)
                return;

            ClockArmed = false;
        }

        public void Start()
        {
            if (ClockArmed) { Controller.MainTask.Start(); }
        }

        public void Stop()
        {
            if (ClockRunning) { Controller.MainTask.Stop(); }
        }
        private void Controller_OnTaskStateChanged(object? sender, NITaskStateEventArgs e)
        {


            if (e.NewState == TaskState.Running)
            {
                ClockRunning = true;
                return;
            }

            ClockRunning = false;
        }
    }
}
