using Leak.Core.Common;
using System;

namespace Leak.Core.Retriever
{
    public class ResourceBitfieldBook
    {
        public PeerHash Peer { get; set; }

        public DateTime Expires { get; set; }

        public ResourceBlock Request { get; set; }
    }
}