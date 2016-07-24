using Leak.Core.Common;
using System;

namespace Leak.Core.Retriever
{
    public class ResourceMetadataBook
    {
        public PeerHash Peer { get; set; }

        public DateTime Expires { get; set; }

        public ResourceMetadataBlock Request { get; set; }
    }
}