using Sardine.Core.Metadata;

namespace Sardine.Recording.Metadata
{
    public static class MetadataExtensions
    {
        public static TomlNode ToToml(this MetadataObject metadataObject)
        {
            switch (metadataObject.Type)
            {
                case MetadataType.TypeString:
                    return (string)metadataObject.Data;
                case MetadataType.TypeInteger:
                    return (int)metadataObject.Data;
                case MetadataType.TypeFloat:
                    return (double)metadataObject.Data;
                case MetadataType.TypeBool:
                    return (bool)metadataObject.Data;
                case MetadataType.TypeDateTimeLocal:
                    return (DateTime)metadataObject.Data;
                case MetadataType.TypeDateTimeOffset:
                    return (DateTimeOffset)metadataObject.Data;
                case MetadataType.TypeArray:
                    return ((IEnumerable<MetadataObject>)metadataObject.Data).Select(x => x.ToToml()).ToArray();
                case MetadataType.TypeTable:
                    TomlTable tomlTable = new();
                    foreach (var kvp in ((IDictionary<string, MetadataObject>)metadataObject.Data))
                        tomlTable.Add(kvp.Key, kvp.Value.ToToml());
                    return tomlTable;
                default:
                    throw new ArgumentOutOfRangeException(nameof(metadataObject));
            }
        }
    }
}