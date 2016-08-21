using Leak.Core.Cando.Metadata;
using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Scheduler
{
    public interface SchedulerTaskCallback
    {
        void OnMetadataSize(PeerHash peer, MetadataSize size);

        void OnMetadataData(PeerHash peer, MetadataData data);

        void OnPeerBitfield(PeerHash peer, Bitfield bitfield);

        void OnPeerPiece(PeerHash peer, Piece piece);
    }
}