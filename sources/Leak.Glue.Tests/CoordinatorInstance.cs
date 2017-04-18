using System;

namespace Leak.Peer.Coordinator.Tests
{
    public class CoordinatorInstance : IDisposable
    {
        private readonly CoordinatorService service;

        public CoordinatorInstance(CoordinatorService service)
        {
            this.service = service;
        }

        public CoordinatorService Service
        {
            get { return service; }
        }

        public void Dispose()
        {
        }
    }
}