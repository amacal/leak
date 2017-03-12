using Leak.Common;
using Leak.Events;

namespace Leak.Meta.Store
{
    public static class MetafileExtensions
    {
        public static void CallMetafileRead(this MetafileHooks hooks, FileHash hash, int piece, int total, byte[] data)
        {
            hooks.OnMetafileRead?.Invoke(new MetafileRead
            {
                Hash = hash,
                Piece = piece,
                Total = total,
                Payload = data
            });
        }

        public static void CallMetafileWritten(this MetafileHooks hooks, FileHash hash, int piece, int size)
        {
            hooks.OnMetafileWritten?.Invoke(new MetafileWritten
            {
                Hash = hash,
                Piece = piece,
                Size = size
            });
        }

        public static void CallMetafileVerified(this MetafileHooks hooks, FileHash hash, Metainfo metainfo, int size)
        {
            hooks.OnMetafileVerified?.Invoke(new MetafileVerified
            {
                Hash = hash,
                Metainfo = metainfo,
                Size = size
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