using Leak.Common;
using Leak.Omnibus.Strategies;
using System.Collections.Generic;

namespace Leak.Omnibus
{
    public abstract class OmnibusStrategy
    {
        public static OmnibusStrategy Sequential = new OmnibusStrategySequential();

        public static OmnibusStrategy RarestFirst = new OmnibusStrategyRarestFirst();

        public abstract void Next(ICollection<BlockIndex> blocks, OmnibusContext context, PeerHash peer, int count);
    }
}