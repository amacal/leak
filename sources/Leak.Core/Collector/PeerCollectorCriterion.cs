using Leak.Core.Common;
using System.Collections.Generic;

namespace Leak.Core.Collector
{
    public interface PeerCollectorCriterion
    {
        IEnumerable<PeerSession> Accept(IEnumerable<PeerSession> sessions, PeerCollectorContext context);
    }
}