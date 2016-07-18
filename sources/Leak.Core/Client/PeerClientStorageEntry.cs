using Leak.Core.Metadata;
using Leak.Core.Repository;
using Leak.Core.Retriever;

namespace Leak.Core.Client
{
    public class PeerClientStorageEntry
    {
        public Metainfo Metainfo { get; set; }

        public ResourceRepository Repository { get; set; }

        public ResourceRetriever Retriever { get; set; }
    }
}