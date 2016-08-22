using Leak.Core.Common;
using Leak.Core.Congestion;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Collector.Criterions
{
    public class IsLocalNotChokedByRemote : PeerCollectorCriterion
    {
        public IEnumerable<PeerSession> Accept(IEnumerable<PeerSession> sessions, PeerCollectorContext context)
        {
            return sessions.Where(session => context.Congestion.IsChoking(session.Peer, PeerCongestionDirection.Remote) == false);
        }
    }
}