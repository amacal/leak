using Leak.Common;
using Leak.Events;

namespace Leak.Meta.Share
{
    public static class MetashareExtensions
    {
        public static void CallMetadataShared(this MetashareHooks hooks, FileHash hash, PeerHash peer, int piece)
        {
            hooks.OnMetadataShared?.Invoke(new MetadataShared
            {
                Hash = hash,
                Peer = peer,
                Piece = piece
            });
        }
    }
}