using Leak.Core.Collector;
using Leak.Core.Repository;

namespace Leak.Core.Retriever
{
    public class ResourceRetriever
    {
        private readonly ResourceRepository repository;
        private readonly PeerCollectorView collector;

        public ResourceRetriever(ResourceRepository repository, PeerCollectorView collector)
        {
            this.repository = repository;
            this.collector = collector;
        }
    }
}