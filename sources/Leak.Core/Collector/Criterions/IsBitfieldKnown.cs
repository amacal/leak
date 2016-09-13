using Leak.Core.Common;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Collector.Criterions
{
    public class IsBitfieldKnown : PeerCollectorCriterion
    {
        public IEnumerable<PeerSession> Accept(IEnumerable<PeerSession> sessions, PeerCollectorContext context)
        {
            return sessions.Where(context.Battlefield.Contains);
        }
    }
}