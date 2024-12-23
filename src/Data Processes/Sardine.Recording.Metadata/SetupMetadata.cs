using Sardine.Core;

namespace Sardine.Recording.Metadata
{
    public class SetupMetadata
    {
        public string Version { get; } = SardineInfo.FleetVersion;
        public string Name { get;  } = SardineInfo.FleetName;

        public List<string> AvailableModules => Fleet.Current.VesselCollection.Where(x => x.IsOnline).Select(x => x.DisplayName).ToList();
        public List<string> LinkActiveModules => Fleet.Current.VesselCollection.Where(x => x.IsActive).Select(x => x.DisplayName).ToList();


        public SetupMetadata() { }
        public virtual void SetFromToml(TomlTable buildTable) { }

        internal virtual TomlTable GetToml()
        {
            TomlTable table = new();
            table[nameof(Version)] = Version;
            table[nameof(Name)] = Name;

            var availableModules = new TomlArray();
            foreach (var moduleEntry in AvailableModules)
                availableModules.Add(moduleEntry);
            table[nameof(AvailableModules)] = availableModules;

            var linkActiveModules = new TomlArray();
            foreach (var moduleEntry in LinkActiveModules)
                linkActiveModules.Add(moduleEntry);
            table[nameof(LinkActiveModules)] = linkActiveModules;

            return table;
        }
    }
}
