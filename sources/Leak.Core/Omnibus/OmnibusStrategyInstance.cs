using Leak.Core.Common;
using System.Collections.Generic;

namespace Leak.Core.Omnibus
{
    public interface OmnibusStrategyInstance
    {
        IEnumerable<OmnibusBlock> Next(OmnibusContext context, PeerHash peer, int count);
    }
}