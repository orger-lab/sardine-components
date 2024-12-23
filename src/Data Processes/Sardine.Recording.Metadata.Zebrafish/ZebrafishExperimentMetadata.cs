using Sardine.Core.Utils.Text;

namespace Sardine.Recording.Metadata.Zebrafish
{
    public class ZebrafishExperimentMetadata : ExperimentMetadata
    {
        public ZebrafishMetadata Fish { get; } = new ZebrafishMetadata();

        public override string GenerateFolderName()
        {
            string folderName = $"{CreatedAt:yyyyMMdd}_"
                                + ((Name.Length == 0) ? "" : $"{StringOperations.FilterToNCharacters(StringOperations.ToTitleCase(Name), 20)}_")
                                + ((Version < 0)?"" :$"v{StringOperations.SwitchSeparators(Version.ToString())}_")
                                + ((Condition.Length == 0) ? "" : $"{StringOperations.FilterToNCharacters(StringOperations.ToTitleCase(Condition), 20)}_")
                                + ((Author.Length == 0) ? "" : $"{StringOperations.FilterToNCharacters(StringOperations.ToTitleCase(Author), 20)}_")
                                + ((Fish.Age.Length == 0) ? "" : $"{StringOperations.FilterToNCharacters(StringOperations.SwitchSeparators(Fish.Age), 20)}_")
                                + ((Fish.Line.Length == 0) ? "" : $"{StringOperations.FilterToNCharacters(StringOperations.SwitchSeparators(Fish.Line,"-"), 20)}_")
                                + $"f{ID}";

            ExperimentFolderName = StringOperations.SanitizeFileName(folderName);

            return folderName;
        }

        public override void SetFromToml(TomlTable buildTable)
        {
            base.SetFromToml(buildTable);
            Fish.SetFromToml(buildTable[nameof(Fish)].AsTable);
        }

        public override TomlTable GetToml()
        {
            TomlTable toml = base.GetToml();
            toml.Add(nameof(Fish), Fish.GetToml());
            return toml;
        }
    }
}
