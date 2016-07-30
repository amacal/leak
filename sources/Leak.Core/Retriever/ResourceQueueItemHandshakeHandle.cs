using Leak.Core.Common;

namespace Leak.Core.Retriever
{
    public class ResourceQueueItemHandshakeHandle : ResourceQueueItem
    {
        private readonly PeerHash peer;

        public ResourceQueueItemHandshakeHandle(PeerHash peer)
        {
            this.peer = peer;
        }

        public void Handle(ResourceQueueContext context)
        {
            context.Collector.SendExtended(peer, context.Extender.GetHandshake());
        }
    }
}