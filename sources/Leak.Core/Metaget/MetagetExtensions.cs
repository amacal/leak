using Leak.Core.Common;
using Leak.Core.Events;
using Leak.Core.Metadata;
using Leak.Core.Metamine;

namespace Leak.Core.Metaget
{
    public static class MetagetExtensions
    {
        public static void CallMetafileMeasured(this MetagetHooks hooks, FileHash hash, int size)
        {
            hooks.OnMetadataMeasured?.Invoke(new MetafileMeasured
            {
                Hash = hash,
                Size = size
            });
        }

        public static void CallMetadataPieceRequested(this MetagetHooks hooks, FileHash hash, PeerHash peer, int piece)
        {
            hooks.OnMetadataPieceRequested?.Invoke(new MetadataPieceRequested
            {
                Hash = hash,
                Peer = peer,
                Piece = piece
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