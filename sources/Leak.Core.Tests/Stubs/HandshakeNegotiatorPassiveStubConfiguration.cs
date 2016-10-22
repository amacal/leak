using System.Collections.Generic;
using Leak.Core.Common;

namespace Leak.Core.Tests.Stubs
{
    public class HandshakeNegotiatorPassiveStubConfiguration
    {
        public List<FileHash> Hashes { get; set; }

        public PeerHash Peer { get; set; }
    }
}