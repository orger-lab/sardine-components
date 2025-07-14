using Microsoft.Win32;
using Sardine.Core.Logs;
using Sardine.Core.Views.WPF;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;


namespace Sardine.Sequencer.Views.WPF
{
    public partial class ExperimentController_UI : VesselUserControl<ExperimentController>
    {

        private ObservableCollection<BlockRepetitionPair> items;
        private BlockRepetitionPair selectedItem = new BlockRepetitionPair();

     
        public int Repetitions
        {
            get => SelectedItem.Repetitions; set
            {
                if (value < 1)
                    value = 1;

                SelectedItem.Repetitions = value;
                if (ListBox_Items.SelectedIndex >= 0)
                    Items[ListBox_Items.SelectedIndex] = new BlockRepetitionPair() { Repetitions = Repetitions, ScheduleItem = ItemType };
                OnPropertyChanged();
            }
        }

        public IPatternBlockProvider ItemType
        {
            get => SelectedItem.ScheduleItem; set
            {
                SelectedItem.ScheduleItem = value;
                if (ListBox_Items.SelectedIndex >= 0)
                    Items[ListBox_Items.SelectedIndex] = new BlockRepetitionPair() { Repetitions = Repetitions, ScheduleItem = ItemType };
                OnPropertyChanged();
            }
        }

        public BlockRepetitionPair SelectedItem
        {
            get => selectedItem; set
            {
                selectedItem.ScheduleItem = value.ScheduleItem;
                selectedItem.Repetitions = value.Repetitions;
                
                OnPropertyChanged();
                OnPropertyChanged(nameof(Repetitions));
                OnPropertyChanged(nameof(ItemType));
            }
        }

        public ObservableCollection<BlockRepetitionPair> Items
        {
            get => items; set
            {
                items = value;
                OnPropertyChanged();
            }
        }

       
        public ExperimentController_UI()
        {
            InitializeComponent();
            Items = new ObservableCollection<BlockRepetitionPair>();
        }

        public override void OnVesselReloadedAction()
        {
            ComboBox_ItemType.ItemsSource = Handle.BlockProviderList;
            ComboBox_ItemType.SelectedIndex = 0;
            ItemType = (IPatternBlockProvider)ComboBox_ItemType.SelectedItem;
        }


        private void Button_Up_Click(object sender, RoutedEventArgs e)
        {
            if (ListBox_Items.SelectedIndex >= 0)
                Items.Move(ListBox_Items.SelectedIndex, Math.Max(0, ListBox_Items.SelectedIndex - 1));
        }

        private void Button_Down_Click(object sender, RoutedEventArgs e)
        {
            if (ListBox_Items.SelectedIndex >= 0)
                Items.Move(ListBox_Items.SelectedIndex, Math.Min(Items.Count - 1, ListBox_Items.SelectedIndex + 1));
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            Items.Add(new BlockRepetitionPair() { Repetitions = Repetitions, ScheduleItem = ItemType });
        }

        private void Button_Remove_Click(object sender, RoutedEventArgs e)
        {
            if (ListBox_Items.SelectedIndex >= 0)
            {
                int index = ListBox_Items.SelectedIndex;
                Items.RemoveAt(ListBox_Items.SelectedIndex);
                ListBox_Items.SelectedIndex = index - 1;
            }
        }

        private void Button_Import_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() ?? false)
            {
                Items.Clear();
                using (StreamReader sr = new StreamReader(ofd.FileName))
                {
                    string line;
                    while(true)
                    {     
                        line = sr.ReadLine();

                        if (line is null)
                            break;

                        var brp = ParseLine(line);

                        if (brp is null)
                        {
                            this.Log("Couldn't parse acquisition schedule.", LogLevel.Error);
                            return;
                        }
                        Items.Add(brp);
                    }
                }
            }
        }

        BlockRepetitionPair ParseLine(string line)
        {
            try
            {
                var splitResult = line.Split(' ');
                return new BlockRepetitionPair() { Repetitions = int.Parse(splitResult[1]), ScheduleItem = Handle!.BlockProviderList.Where(x => x.ShortName == splitResult[0]).First() };
            }
            catch
            {
                return null;
            }
        }

        private void Button_Export_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog();
            if (sfd.ShowDialog() ?? false)
            {
                using (StreamWriter writer = new StreamWriter(sfd.FileName))
                {
                    foreach (BlockRepetitionPair item in Items)
                    {
                        writer.WriteLine($"{item.ScheduleItem.ShortName} {item.Repetitions}");
                    }
                }
            }
        }

        private void Button_Arm_Click(object sender, RoutedEventArgs e)
        {
            if (Handle is not null && !Handle.Sequencer.IsRunning)
            {
                if (Handle.Sequencer.IsArmed)
                {
                    Handle.Sequencer.Disarm();
                }
                else
                {
                    if (Items.Count > 0)
                    {
                        Handle.BlockSequence = Items.Select(x => (x.Repetitions, x.ScheduleItem)).ToList();
                        Handle.ArmSequencer();
                    }
                }
            }
        }
    }
}
    
