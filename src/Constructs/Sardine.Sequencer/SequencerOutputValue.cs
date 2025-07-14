namespace Sardine.Sequencer
{
    public class SequencerOutputValue
    {
        public SequencerOutputValue(double analog)
        {
            Analog = analog;
        }

        public SequencerOutputValue(bool digital)
        {
            Digital = digital;
        }

        public SequencerOutputValue(string? virtualOutput)
        {
            Virtual = virtualOutput;
        }

        public SequencerOutputValue() { }



        public double? Analog { get; }
        public bool? Digital { get; }
        public string? Virtual { get; }

        public override string ToString()
        {
            if (Analog is not null)
                return $"{Analog:0.00000}";

            if (Digital is not null)
                return $"{((bool)Digital ? 1 : 0)}";

            return Virtual ?? string.Empty;
        }
    }

}
