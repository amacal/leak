using Leak.Common;
using Leak.Core.Omnibus.Strategies;
using System.Collections.Generic;

namespace Leak.Core.Omnibus
{
    public abstract class OmnibusStrategy
    {
        public static OmnibusStrategy Sequential = new OmnibusStrategySequential();

        public static OmnibusStrategy RarestFirst = new OmnibusStrategyRarestFirst();

        public abstract void Next(ICollection<OmnibusBlock> blocks, OmnibusContext context, PeerHash peer, int count);
    }
}