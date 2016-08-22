using Leak.Core.Cando.Metadata;
using Leak.Core.Common;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Collector.Criterions
{
    public class IsMetadataSupportedByRemote : PeerCollectorCriterion
    {
        public IEnumerable<PeerSession> Accept(IEnumerable<PeerSession> sessions, PeerCollectorContext context)
        {
            return sessions.Where(session => context.Cando.Supports(session, formatter => formatter.DoesSupportMetadata()));
        }
    }
}