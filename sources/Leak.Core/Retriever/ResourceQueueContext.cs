using Leak.Core.Collector;
using Leak.Core.Extensions;
using Leak.Core.Repository;

namespace Leak.Core.Retriever
{
    public class ResourceQueueContext
    {
        public ResourceStorage Storage { get; set; }

        public PeerCollectorView Collector { get; set; }

        public Extender Extender { get; set; }

        public ResourceRetrieverCallback Callback { get; set; }

        public ResourceRepository Repository { get; set; }
    }
}