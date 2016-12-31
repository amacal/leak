using System;
using Leak.Common;

namespace Leak.Leakage.Tests
{
    public class LeakageSwarm : IDisposable
    {
        private readonly FileHash hash;
        private readonly LeakageNode sue;
        private readonly LeakageNode bob;
        private readonly LeakageNode joe;

        public LeakageSwarm(FileHash hash, LeakageNode sue, LeakageNode bob, LeakageNode joe)
        {
            this.hash = hash;
            this.sue = sue;
            this.bob = bob;
            this.joe = joe;
        }

        public LeakageNode Sue
        {
            get { return sue; }
        }

        public LeakageNode Bob
        {
            get { return bob; }
        }

        public LeakageNode Joe
        {
            get { return joe; }
        }

        public FileHash Hash
        {
            get { return hash; }
        }

        public void Dispose()
        {
            sue.Dispose();
            bob.Dispose();
            joe.Dispose();
        }
    }
}
