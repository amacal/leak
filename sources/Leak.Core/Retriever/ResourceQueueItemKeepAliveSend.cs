namespace Leak.Core.Retriever
{
    public class ResourceQueueItemKeepAliveSend : ResourceQueueItem
    {
        public void Handle(ResourceQueueContext context)
        {
            foreach (ResourcePeer peer in context.Storage.GetPeers(ResourcePeerOperation.KeepAlive))
            {
                context.Collector.SendKeepAlive(peer.Hash);
            }
        }
    }
}