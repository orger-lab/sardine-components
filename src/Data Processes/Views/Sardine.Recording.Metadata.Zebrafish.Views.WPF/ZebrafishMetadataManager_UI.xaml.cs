using Sardine.Core.Views.WPF;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using Sardine.Recording.Metadata.Zebrafish;

namespace Sardine.Recording.Metadata.Views.WPF
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ZebrafishMetadataManager_UI : VesselUserControl<ZebrafishExperimentMetadataController>
    {
        //private string basePath = string.Empty;
        //private string folderName = string.Empty;

        //public string BasePath
        //{
        //    get => basePath; set
        //    {
        //        basePath = value;
        //        OnPropertyChanged();
        //        UpdateMetadataFolder();
        //    }
        //}

        //private void UpdateMetadataFolder()
        //{
        //    if (Handle is not null)
        //    {
        //        Handle.Metadata.ExperimentFolderBase = Path.Combine(BasePath, FolderName);
        //        Handle.Metadata.ExperimentFolderName = FolderName;
        //    }
        //}

        //public string FolderName
        //{
        //    get => folderName; set
        //    {
        //        folderName = value;
        //        OnPropertyChanged();
        //        UpdateMetadataFolder();
        //    }
        //}


        //private ObservableCollection<ProtocolItem> items;
        //private ProtocolItem selectedItem = new (string.Empty, string.Empty);

        //public string ProtocolItemPath
        //{
        //    get => SelectedItem.Path; set
        //    {

        //        SelectedItem.Path = value;
        //        if (ListBox_Items.SelectedIndex >= 0)
        //            Items[ListBox_Items.SelectedIndex] = new ProtocolItem(ProtocolItemName,ProtocolItemPath);
        //        OnPropertyChanged();
        //    }
        //}

        //public string ProtocolItemName
        //{
        //    get => SelectedItem.Name; set
        //    {
        //        SelectedItem.Name = value;
        //        if (ListBox_Items.SelectedIndex >= 0)
        //            Items[ListBox_Items.SelectedIndex] = new ProtocolItem(ProtocolItemName, ProtocolItemPath);
        //        OnPropertyChanged();
        //    }
        //}

        //public ProtocolItem SelectedItem
        //{
        //    get => selectedItem; set
        //    {
        //        if (value != null)
        //        {
        //            selectedItem.Name = value.Name;
        //            selectedItem.Path = value.Path;
        //        }
        //        //selectedItem = value;
        //        OnPropertyChanged();
        //        OnPropertyChanged(nameof(ProtocolItemPath));
        //        OnPropertyChanged(nameof(ProtocolItemName));
        //    }
        //}

        //public ObservableCollection<ProtocolItem> Items
        //{
        //    get => items; set
        //    {
        //        items = value;
        //        OnPropertyChanged();
        //    }
        //}

        public override void OnVesselReloadedAction()
        {
       //     UpdateProtocolList();
        }



        public ZebrafishMetadataManager_UI()
        {
            InitializeComponent();
            // items = new ObservableCollection<ProtocolItem>();
        }

        private void Button_SetBasePath_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFolderDialog();

            if (ofd.ShowDialog() ?? false && Handle is not null)
            {
                //ExecuteCall((metadataManager)=> { metadataManager.Metadata.ExperimentFolderBase = ofd.FolderName; });
                Handle!.Metadata.ExperimentFolderBase = ofd.FolderName;
            }
        }

        private void Button_LoadTemplate_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() ?? false)
            {
                Handle?.LoadExperiment(Path.GetDirectoryName(ofd.FileName)!,Path.GetFileName(ofd.FileName));
            }
    //        UpdateProtocolList();
        }

        //private void UpdateProtocolList()
        //{
            
        //    Items = [];
        //    if (Handle is not null)
        //    {
        //        foreach (var item in Handle.Metadata.Protocols)
        //        {
        //            Items.Add(new ProtocolItem(item.Key, item.Value));
        //        }
        //    }
        //}

        //private void Button_AddProtocol_Click(object sender, RoutedEventArgs e)
        //{
        //    Items.Add(new ProtocolItem(ProtocolItemName, ProtocolItemPath));
        //}

        //private void Button_RemoveProtocol_Click(object sender, RoutedEventArgs e)
        //{
        //    if (ListBox_Items.SelectedIndex >= 0)
        //        Items.RemoveAt(ListBox_Items.SelectedIndex);
        //}

        private void Button_ExportMetadata_Click(object sender, RoutedEventArgs e)
        {
            if (Handle is not null)
            {
                if (Directory.Exists(Path.Combine(Handle.Metadata.ExperimentFolderBase, Handle.Metadata.ExperimentFolderName)))
                {
                    MessageBoxResult result = MessageBox.Show("Experiment folder already exists. Overwrite and continue?", "Folder already exists", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.No)
                        return;
                }

              //  foreach (ProtocolItem item in Items)
             //   {
                    //if 
               //     Handle.Metadata.Protocols.TryAdd(item.Name, item.Path);
                   // {
                   // Logger.Log(new LogMessage("Multiple protocols exist with the same receiverID!", LogLevel.Error));   
                   //  return;
                   // }
                   //Manager.RemoteController.ExecuteCall(item.Name, "SetProtocol", item.Name);
            //    }
                
                Handle.CollectAndSaveMetadata();
            }
        }

        //private void Button_FilenameProtocol_Click(object sender, RoutedEventArgs e)
        //{
        //    var ofd = new OpenFileDialog();
        //    if (ofd.ShowDialog() ?? false)
        //    {
        //        ProtocolItemPath = ofd.FileName;
        //    }
        //}

        public class ProtocolItem(string name, string path)
        {
            public string Name { get; set; } = name;
            public string Path { get; set; } = path;

            public override string ToString() => $"[{Name}] {Path}";
        }

        private void Button_GenerateFolderName_Click(object sender, RoutedEventArgs e)
        {
            Handle?.Metadata.GenerateFolderName();
        }
    }

}
