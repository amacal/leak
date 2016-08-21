using Leak.Core.Common;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Collector.Criterions
{
    public class IsBitfieldKnown : PeerCollectorCriterion
    {
        public IEnumerable<PeerHash> Accept(IEnumerable<PeerHash> peers, PeerCollectorContext context)
        {
            return peers.Where(context.Battlefield.Contains);
        }
    }
}