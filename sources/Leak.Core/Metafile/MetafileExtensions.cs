using Leak.Core.Common;
using Leak.Core.Events;
using Leak.Core.Metadata;

namespace Leak.Core.Metafile
{
    public static class MetafileExtensions
    {
        public static void CallMetadataDiscovered(this MetafileHooks hooks, FileHash hash, Metainfo metainfo)
        {
            hooks.OnMetadataDiscovered?.Invoke(new MetadataDiscovered
            {
                Hash = hash,
                Metainfo = metainfo
            });
        }
    }
}