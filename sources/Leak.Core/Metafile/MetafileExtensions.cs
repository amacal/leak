using Leak.Core.Common;
using Leak.Core.Events;
using Leak.Core.Metadata;

namespace Leak.Core.Metafile
{
    public static class MetafileExtensions
    {
        public static void CallMetafileWritten(this MetafileHooks hooks, FileHash hash, int piece, int size)
        {
            hooks.OnMetafileWritten?.Invoke(new MetafileWritten
            {
                Hash = hash,
                Piece = piece,
                Size = size
            });
        }

        public static void CallMetafileVerified(this MetafileHooks hooks, FileHash hash, Metainfo metainfo)
        {
            hooks.OnMetafileVerified?.Invoke(new MetafileVerified
            {
                Hash = hash,
                Metainfo = metainfo
            });
        }

        public static void CallMetafileRejected(this MetafileHooks hooks, FileHash hash)
        {
            hooks.OnMetafileRejected?.Invoke(new MetafileRejected
            {
                Hash = hash
            });
        }
    }
}