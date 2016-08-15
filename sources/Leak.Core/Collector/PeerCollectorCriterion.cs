using Leak.Core.Common;
using System.Collections.Generic;

namespace Leak.Core.Collector
{
    public interface PeerCollectorCriterion
    {
        IEnumerable<PeerHash> Accept(IEnumerable<PeerHash> peers, PeerCollectorContext context);
    }
}