namespace Sardine.Recording.Metadata.Zebrafish
{
    public class ZebrafishMetadata
    {
        public string Line { get; set; } = string.Empty;
        public string Tank { get; set; } = string.Empty;
        public string Age { get; set; } = string.Empty;

        public void SetFromToml(TomlTable buildTable)
        {
            Line = buildTable[nameof(Line)];
            Tank = buildTable[nameof(Tank)];
            Age = buildTable[nameof(Age)];
        }
        public TomlTable GetToml()
        {
            TomlTable tomlTable = new()
            {
                { nameof(Line), Line },
                { nameof(Tank), Tank },
                { nameof(Age), Age }
            };

            return tomlTable;
        }
    }
}
