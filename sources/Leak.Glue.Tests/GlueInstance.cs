using System;

namespace Leak.Peer.Coordinator.Tests
{
    public class GlueInstance : IDisposable
    {
        private readonly GlueService service;

        public GlueInstance(GlueService service)
        {
            this.service = service;
        }

        public GlueService Service
        {
            get { return service; }
        }

        public void Dispose()
        {
        }
    }
}