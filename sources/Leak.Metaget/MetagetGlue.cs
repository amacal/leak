using System.Collections.Generic;
using Leak.Common;

namespace Leak.Meta.Get
{
    public interface MetagetGlue
    {
        IEnumerable<PeerHash> Peers { get; }

        void SendMetadataRequest(PeerHash peer, int piece);
    }
}