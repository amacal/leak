using System.Collections.Generic;
using Leak.Common;
using Leak.Data.Map.Strategies;

namespace Leak.Data.Map
{
    public abstract class OmnibusStrategy
    {
        public static OmnibusStrategy Sequential = new OmnibusStrategySequential();

        public static OmnibusStrategy RarestFirst = new OmnibusStrategyRarestFirst();

        public abstract void Next(ICollection<BlockIndex> blocks, OmnibusContext context, PeerHash peer, int count);
    }
}