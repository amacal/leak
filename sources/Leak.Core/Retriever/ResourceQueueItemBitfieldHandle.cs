using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Retriever
{
    public class ResourceQueueItemBitfieldHandle : ResourceQueueItem
    {
        private readonly PeerHash peer;
        private readonly Bitfield bitfield;

        public ResourceQueueItemBitfieldHandle(PeerHash peer, Bitfield bitfield)
        {
            this.peer = peer;
            this.bitfield = bitfield;
        }

        public void Handle(ResourceQueueContext context)
        {
            context.Storage.AddPeer(peer);
            context.Storage.AddBitfield(peer, bitfield);
            context.Collector.SendInterested(peer);
        }
    }
}