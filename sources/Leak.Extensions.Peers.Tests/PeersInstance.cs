using System;
using Leak.Peer.Coordinator;

namespace Leak.Extensions.Peers.Tests
{
    public class PeersInstance : IDisposable
    {
        private readonly GlueService service;

        public PeersInstance(GlueService service)
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