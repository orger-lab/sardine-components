namespace Sardine.Recording.Metadata
{
    public class OnMetadataCollectedEventArgs(ExperimentMetadata metadata)
    {
        public ExperimentMetadata Metadata { get; } = metadata;

    }
}
