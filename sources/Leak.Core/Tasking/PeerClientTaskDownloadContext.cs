using Leak.Core.Messages;
using Leak.Core.Metadata;
using Leak.Core.Retriever;

namespace Leak.Core.Tasking
{
    public class PeerClientTaskDownloadContext
    {
        public Metainfo Metainfo { get; set; }

        public string Destination { get; set; }

        public Bitfield Bitfield { get; set; }

        public RetrieverService Retriever { get; set; }

        public PeerClientTaskQueue Queue { get; set; }

        public PeerClientTask Task { get; set; }
    }
}