using Leak.Core.Common;

namespace Leak.Core.Retriever
{
    public class ResourceQueueItemChokeHandle : ResourceQueueItem
    {
        private readonly PeerHash peer;

        public ResourceQueueItemChokeHandle(PeerHash peer)
        {
            this.peer = peer;
        }

        public void Handle(ResourceQueueContext context)
        {
            context.Storage.Choke(peer);
        }
    }
}