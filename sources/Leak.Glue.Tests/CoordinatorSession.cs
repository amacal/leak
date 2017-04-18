using System;
using Leak.Common;

namespace Leak.Peer.Coordinator.Tests
{
    public class CoordinatorSession : IDisposable
    {
        private readonly CoordinatorService coordinator;

        public CoordinatorSession(CoordinatorService coordinator)
        {
            this.coordinator = coordinator;
        }

        public CoordinatorService Coordinator
        {
            get { return coordinator; }
        }

        public void Dispose()
        {
        }
    }
}