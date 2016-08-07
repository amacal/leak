using System.Collections.Generic;

namespace Leak.Core.Omnibus
{
    public abstract class OmnibusStrategy
    {
        public static OmnibusStrategy Sequential = new OmnibusStrategySequential();

        public abstract IEnumerable<OmnibusBlock> Next(OmnibusStrategyContext context, int count);
    }
}