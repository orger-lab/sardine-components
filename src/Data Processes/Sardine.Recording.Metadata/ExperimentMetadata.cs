using Sardine.Core;
using Sardine.Core.Utils.Text;

namespace Sardine.Recording.Metadata
{
    public class ExperimentMetadata
    {
        public static string DefaultFilename = "metadata.toml";

        public string ExperimentFolderBase { get; set; } = "";
        public string MetadataFilename { get; set; } = DefaultFilename;
        public string ExperimentFolderName { get; set; } = "";

        public Dictionary<string, string> Input { get; set; } = [];
        public List<string> Output { get; set; } = [];

        public string SardineVersion { get; } = SardineInfo.SardineVersion;
        public decimal Version { get; set; } = -1;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Notes { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public int ID { get; set; } = 0;
        public SetupMetadata Setup { get; } = new();

        public virtual string GenerateFolderName()
        {
            string folderName = $"{CreatedAt:yyyyMMdd}_"
                                + $"{StringOperations.FilterToNCharacters(StringOperations.ToTitleCase(Name), 20)}_"
                                + $"v{StringOperations.SwitchSeparators(Version.ToString())}_"
                                + $"{StringOperations.FilterToNCharacters(StringOperations.ToTitleCase(Condition), 20)}_"
                                + $"{ID}";

            ExperimentFolderName = StringOperations.SanitizeFileName(folderName);
            return folderName;
        }

        public virtual void SetFromToml(TomlTable buildTable)
        {
            Version = buildTable[nameof(Version)];
            Name = buildTable[nameof(Name)];
            Description = buildTable[nameof(Description)];
            Author = buildTable[nameof(Author)];
            //CreatedAt = buildTable[nameof(CreatedAt)];
            Notes = buildTable[nameof(Notes)];
            Condition = buildTable[nameof(Condition)];
            ID = buildTable[nameof(ID)];
            Setup.SetFromToml(buildTable[nameof(Setup)].AsTable);
            Input = [];
            //if (buildTable.HasKey(nameof(Protocols)))
            //    foreach (var entry in buildTable[nameof(Protocols)].AsTable.Keys)
            //        Protocols[entry] = buildTable[nameof(Protocols)].AsTable[entry];

            Output = [];
        }

        public virtual TomlTable GetToml()
        {
            TomlTable table = new TomlTable();
            table[nameof(ExperimentFolderBase)] = ExperimentFolderBase;
            table[nameof(ExperimentFolderName)] = ExperimentFolderName;
            table[nameof(Description)] = Description;
            table[nameof(Author)] = Author;
            table[nameof(CreatedAt)]= CreatedAt;
            table[nameof(Notes)] = Notes;
            table[nameof(Condition)] = Condition;
            table[nameof(ID)] = ID;
            table[nameof(Version)] = (double)Version;
            table[nameof(Name)] = Name;
            table[nameof(SardineVersion)] = SardineVersion;

            var protocols = new TomlTable();
            foreach(var protocolEntry in Input)
                protocols[protocolEntry.Key] = protocolEntry.Value;
            table[nameof(Input)] = protocols;

            var outputs = new TomlArray();
            foreach (var outputEntry in Output)
                outputs.Add(outputEntry);
            table[nameof(Output)] = outputs;

            table[nameof(Setup)] = Setup.GetToml();

            return table;
        }
    }
}
