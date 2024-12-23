using Sardine.Core;
using Sardine.Core.Logs;
using Sardine.Core.Metadata;

namespace Sardine.Recording.Metadata
{
    public class ExperimentMetadataController<T> where T : ExperimentMetadata, new()
    {
        internal Dictionary<Vessel, Action<object, string, string>> PathSettingActions = [];
        internal Dictionary<Vessel, Func<object, IReadOnlyList<ProtocolInfo>>> ProtocolExportDelegates = [];

        public void LoadExperiment(string templateFolder, string? metadataFilename = null)
        {
            if (!Directory.Exists(templateFolder))
            {
                Fleet.Current.Logger!.Log(new LogMessage($"Template folder {templateFolder} does not exist.", LogLevel.Error), this);
                return;
            }
            IEnumerable<string> files = Directory.EnumerateFiles(templateFolder);
            if (!files.Any())
            {
                Fleet.Current.Logger!.Log(new LogMessage($"No template file found in {templateFolder}.", LogLevel.Error), this);
                return;
            }
            string filename = Path.Combine(templateFolder, metadataFilename ?? ExperimentMetadata.DefaultFilename);
            if (files.Count() == 1)
            {
                filename = files.First();
            }

            TomlTable tomlOutput;
            try
            {
                using TextReader reader = new StreamReader(filename);
                tomlOutput = TOML.Parse(reader);
            }
            catch (FileNotFoundException)
            {
                Fleet.Current.Logger!.Log(new LogMessage($"Couldn't find file {Path.GetFileName(filename)}", LogLevel.Error), this);
                return;
            }
            catch (TomlParseException exception)
            {
                Fleet.Current.Logger!.Log(new LogMessage($"Couldn't parse file {Path.GetFileName(filename)}", LogLevel.Error), this);
                foreach ( var error in exception.SyntaxErrors)
                    Fleet.Current.Logger!.Log(new LogMessage($"Line {error.Line}: {error.Message}", LogLevel.Error),this);
                return;
            }

            Metadata.SetFromToml(tomlOutput);
            //Metadata.ExperimentFolder = ExperimentFolder;
            Metadata.MetadataFilename = Path.GetFileName(filename);

           
        }

        public void ExportTemplate(string exportPath)
        {
            TomlTable tomlFromMetadata = Metadata.GetToml();
            string metadataFileContent = tomlFromMetadata.ToString();
            using TextWriter writer = new StreamWriter(exportPath, false);
            writer.Write(metadataFileContent);
        }

        public void CollectAndSaveMetadata()
        {
            Directory.CreateDirectory(Path.Combine(Metadata.ExperimentFolderBase, Metadata.ExperimentFolderName));
            Directory.CreateDirectory(Path.Combine(Metadata.ExperimentFolderBase, Metadata.ExperimentFolderName, nameof(Metadata.Input)));
            Directory.CreateDirectory(Path.Combine(Metadata.ExperimentFolderBase, Metadata.ExperimentFolderName, nameof(Metadata.Output)));

            foreach (KeyValuePair<Vessel, Func<object, IReadOnlyList<ProtocolInfo>>> kvp in ProtocolExportDelegates)
            {
                IReadOnlyList<ProtocolInfo> exportedProtocols = kvp.Key.ExecuteCall(kvp.Value) ?? [];
                
                foreach (ProtocolInfo protocol in exportedProtocols)
                    Metadata.Input[$"{kvp.Key.Name} - {protocol.Name}"] = protocol.Path;
            }


            foreach (var protocol in Metadata.Input)
            {
                if (File.Exists(protocol.Value))
                    File.Copy(protocol.Value, Path.Combine(Metadata.ExperimentFolderBase, Metadata.ExperimentFolderName, nameof(Metadata.Input), protocol.Key+Path.GetExtension(protocol.Value)), true);
            }


            IDictionary<string, MetadataTable> setupMetadata = Fleet.Current.Get<MetadataCollectionService>().CollectMetadata();

            foreach (KeyValuePair<Vessel, Action<object, string, string>> kvp in PathSettingActions)
            {
                if (kvp.Key.ObjectHandle is not null)
                {
                    string pathToSet = Path.Combine(Metadata.ExperimentFolderBase, Metadata.ExperimentFolderName, nameof(Metadata.Output), kvp.Key.Name);
                    
                    if (!Metadata.Output.Contains(kvp.Key.Name))
                        Metadata.Output.Add(kvp.Key.Name);

                    Directory.CreateDirectory(pathToSet);
                    kvp.Key.ExecuteCall((x) => kvp.Value(x, pathToSet, Path.GetFileName(Metadata.ExperimentFolderName)!));
                }
            }

            TomlTable tomlFromMetadata = Metadata.GetToml();

            foreach (KeyValuePair<string, MetadataTable> kvp in setupMetadata)
            {
                TomlNode table = kvp.Value.ToToml();

                //if (table.HasKey("Output"))
                //{
                //    tomlFromMetadata["Output"].AsTable.Add(kvp.Key, table["Output"]);
                //    table.Delete("Output");
                //}
                tomlFromMetadata[nameof(Metadata.Setup)].AsTable.Add(kvp.Key, table);
            }

            //using StringWriter stringWriter = new();

            //x

            using TextWriter writer = new StreamWriter(Path.Combine(Metadata.ExperimentFolderBase, Metadata.ExperimentFolderName, Metadata.MetadataFilename), false);
            tomlFromMetadata.WriteTo(writer);
            //writer.Write(metadataFileContent);

            OnMetadataCollected?.Invoke(this, new OnMetadataCollectedEventArgs(Metadata));


        }


        public event EventHandler<OnMetadataCollectedEventArgs>? OnMetadataCollected;

        public virtual T Metadata { get; } = new();
    }
}
