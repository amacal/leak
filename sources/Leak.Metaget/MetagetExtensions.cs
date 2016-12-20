using Leak.Common;
using Leak.Events;

namespace Leak.Metaget
{
    public static class MetagetExtensions
    {
        public static void CallMetafileMeasured(this MetagetHooks hooks, FileHash hash, int size)
        {
            hooks.OnMetafileMeasured?.Invoke(new MetafileMeasured
            {
                Hash = hash,
                Size = size
            });
        }

        public static void CallMetadataDiscovered(this MetagetHooks hooks, FileHash hash, Metainfo metainfo)
        {
            hooks.OnMetadataDiscovered?.Invoke(new MetadataDiscovered
            {
                Hash = hash,
                Metainfo = metainfo
            });
        }

        public static void Complete(this MetamineBitfield bitfield, int block, int size)
        {
            bitfield.Complete(new MetamineBlock(block, size));
        }
    }
}