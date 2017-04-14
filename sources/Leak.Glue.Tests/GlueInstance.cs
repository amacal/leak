using System;

namespace Leak.Peer.Coordinator.Tests
{
    public class GlueInstance : IDisposable
    {
        private readonly CoordinatorService service;

        public GlueInstance(CoordinatorService service)
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