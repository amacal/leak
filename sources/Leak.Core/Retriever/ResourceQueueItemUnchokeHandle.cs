using Leak.Core.Common;

namespace Leak.Core.Retriever
{
    public class ResourceQueueItemUnchokeHandle : ResourceQueueItem
    {
        private readonly PeerHash peer;

        public ResourceQueueItemUnchokeHandle(PeerHash peer)
        {
            this.peer = peer;
        }

        public void Handle(ResourceQueueContext context)
        {
            context.Storage.Unchoke(peer);
        }
    }
}