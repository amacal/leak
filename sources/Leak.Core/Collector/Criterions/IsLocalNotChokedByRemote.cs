using Leak.Core.Common;
using Leak.Core.Congestion;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Collector.Criterions
{
    public class IsLocalNotChokedByRemote : PeerCollectorCriterion
    {
        public IEnumerable<PeerHash> Accept(IEnumerable<PeerHash> peers, PeerCollectorContext context)
        {
            return peers.Where(peer => context.Congestion.IsChoking(peer, PeerCongestionDirection.Remote) == false);
        }
    }
}