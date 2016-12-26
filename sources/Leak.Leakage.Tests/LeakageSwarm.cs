using System;

namespace Leak.Leakage.Tests
{
    public class LeakageSwarm : IDisposable
    {
        private readonly LeakageNode sue;
        private readonly LeakageNode bob;

        public LeakageSwarm(LeakageNode sue, LeakageNode bob)
        {
            this.sue = sue;
            this.bob = bob;
        }

        public LeakageNode Sue
        {
            get { return sue; }
        }

        public LeakageNode Bob
        {
            get { return bob; }
        }

        public void Dispose()
        {
            sue.Dispose();
            bob.Dispose();
        }
    }
}
