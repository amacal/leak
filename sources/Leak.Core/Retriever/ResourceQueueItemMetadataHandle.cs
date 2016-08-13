using Leak.Core.Cando.Metadata;
using Leak.Core.Common;

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
            ResourceMetadataBlock block = new ResourceMetadataBlock(data.Piece);

            if (context.Storage.IsMetadataComplete() == false)
            {
                context.Storage.Complete(peer, block, data.Size);

                if (context.Repository.SetMetadata(data.Piece, data.Payload))
                {
                    context.Storage.Complete(data.Size);
                    context.Callback.OnMetadataCompleted();
                }
            }
        }
    }
}