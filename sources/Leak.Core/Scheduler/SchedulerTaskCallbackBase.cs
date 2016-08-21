using Leak.Core.Cando.Metadata;
using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Scheduler
{
    public abstract class SchedulerTaskCallbackBase : SchedulerTaskCallback
    {
        public virtual void OnMetadataSize(PeerHash peer, MetadataSize size)
        {
        }

        public virtual void OnMetadataData(PeerHash peer, MetadataData data)
        {
        }

        public virtual void OnPeerBitfield(PeerHash peer, Bitfield bitfield)
        {
        }

        public virtual void OnPeerPiece(PeerHash peer, Piece piece)
        {
        }
    }
}