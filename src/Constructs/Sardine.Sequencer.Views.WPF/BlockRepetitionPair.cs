using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace Sardine.Sequencer.Views.WPF
{
    public class BlockRepetitionPair : INotifyPropertyChanged
    {
        private IPatternBlockProvider scheduleItem;
        private int repetitions = 1;

        public event PropertyChangedEventHandler PropertyChanged;
        public IPatternBlockProvider ScheduleItem
        {
            get => scheduleItem; set
            {
                scheduleItem = value;
                OnPropertyChanged();
            }
        }
        public int Repetitions
        {
            get => repetitions; set
            {
                repetitions = value;
                OnPropertyChanged();
            }
        }
        public void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public override string ToString()
        {
            return $"{ScheduleItem.BlockName} x {Repetitions}";
        }
    }
}
    
