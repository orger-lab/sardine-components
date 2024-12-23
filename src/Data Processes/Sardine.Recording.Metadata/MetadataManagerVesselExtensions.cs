using Sardine.Core;

namespace Sardine.Recording.Metadata
{
    public static class MetadataManagerVesselExtensions
    {
        public static void RegisterSavePathAction<T,K,Y>(this Vessel<T> vessel, Action<T, string, string> action) where T : class where K : ExperimentMetadataController<Y>, new() where Y:ExperimentMetadata,new()
        {
            K manager = Fleet.Current.Get<K>();
            manager.PathSettingActions[vessel] = (x,p,f) => action((x as T)!, p,f);
        }

        public static void RegisterProtocolExportFunction<T, K, Y>(this Vessel<T> vessel, Func<T, IReadOnlyList<ProtocolInfo>> function) where T : class where K : ExperimentMetadataController<Y>, new() where Y : ExperimentMetadata, new()
        {
            K manager = Fleet.Current.Get<K>();
            manager.ProtocolExportDelegates[vessel] = (x) => function((x as T)!);
        }
    }
}
