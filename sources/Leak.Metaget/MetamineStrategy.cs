using System.Collections.Generic;

namespace Leak.Metaget
{
    public abstract class MetamineStrategy
    {
        public static MetamineStrategy Sequential = new MetamineStrategySequential();

        public abstract IEnumerable<MetamineBlock> Next(MetamineStrategyContext context);
    }
}