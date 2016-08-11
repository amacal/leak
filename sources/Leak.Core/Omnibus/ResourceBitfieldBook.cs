using System;
using Leak.Core.Common;
using Leak.Core.Retriever;

namespace Leak.Core.Omnibus
{
    public class OmnibusBitfieldBook
    {
        public PeerHash Peer { get; set; }

        public DateTime Expires { get; set; }

        public ResourceBlock Request { get; set; }
    }
}