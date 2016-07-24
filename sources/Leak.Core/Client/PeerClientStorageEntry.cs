using Leak.Core.Common;
using Leak.Core.Extensions;
using Leak.Core.Metadata;
using Leak.Core.Repository;
using Leak.Core.Retriever;
using System.Collections.Generic;

namespace Leak.Core.Client
{
    public class PeerClientStorageEntry
    {
        public FileHash Hash { get; set; }

        public Metainfo Metainfo { get; set; }

        public ResourceRepository Repository { get; set; }

        public ResourceRetriever Retriever { get; set; }

        public Extender Extender { get; set; }

        public HashSet<PeerHash> Peers { get; set; }
    }
}