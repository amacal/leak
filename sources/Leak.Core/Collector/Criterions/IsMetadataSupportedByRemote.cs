using Leak.Core.Cando.Metadata;
using Leak.Core.Common;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Collector.Criterions
{
    public class IsMetadataSupportedByRemote : PeerCollectorCriterion
    {
        public IEnumerable<PeerHash> Accept(IEnumerable<PeerHash> peers, PeerCollectorContext context)
        {
            return peers.Where(peer => context.Cando.Supports(peer, formatter => formatter.DoesSupportMetadata()));
        }
    }
}