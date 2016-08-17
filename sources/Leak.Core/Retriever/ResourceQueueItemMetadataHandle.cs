using Leak.Core.Cando.Metadata;
using Leak.Core.Common;
using Leak.Core.Metamine;

namespace Leak.Core.Retriever
{
    public class ResourceQueueItemMetadataHandle : ResourceQueueItem
    {
        private readonly PeerHash peer;
        private readonly MetadataData data;

        public ResourceQueueItemMetadataHandle(PeerHash peer, MetadataData data)
        {
            this.peer = peer;
            this.data = data;
        }

        public void Handle(ResourceQueueContext context)
        {
            if (context.Metamine != null)
            {
                MetamineBlock block = new MetamineBlock(data.Block, data.Size);

                context.Metamine.Complete(block);

                if (context.Repository.SetMetadata(data.Block, data.Payload))
                {
                    context.Callback.OnMetadataCompleted();
                }
            }
        }
    }
}