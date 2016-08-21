using Leak.Core.Collector;
using Leak.Core.Messages;
using Leak.Core.Metadata;

namespace Leak.Core.Retriever
{
    public class RetrieverConfiguration
    {
        public Metainfo Metainfo { get; set; }

        public string Destination { get; set; }

        public Bitfield Bitfield { get; set; }

        public PeerCollectorView Collector { get; set; }

        public RetrieverCallback Callback { get; set; }
    }
}