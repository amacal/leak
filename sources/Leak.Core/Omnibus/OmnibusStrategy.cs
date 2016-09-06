using Leak.Core.Common;
using System.Collections.Generic;
using Leak.Core.Omnibus.Components;
using Leak.Core.Omnibus.Strategies;

namespace Leak.Core.Omnibus
{
    public abstract class OmnibusStrategy
    {
        public static OmnibusStrategy Sequential = new OmnibusStrategySequential();

        public static OmnibusStrategy RarestFirst = new OmnibusStrategyRarestFirst();

        public abstract IEnumerable<OmnibusBlock> Next(OmnibusContext context, PeerHash peer, int count);
    }
}