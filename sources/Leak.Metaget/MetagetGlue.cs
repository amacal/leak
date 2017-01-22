using Leak.Common;
using System.Collections.Generic;

namespace Leak.Metaget
{
    public interface MetagetGlue
    {
        IEnumerable<PeerHash> Peers { get; }

        void SendMetadataRequest(PeerHash peer, int piece);
    }
}