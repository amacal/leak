using System;
using Leak.Peer.Coordinator;

namespace Leak.Extensions.Peers.Tests
{
    public class PeersInstance : IDisposable
    {
        private readonly CoordinatorService service;

        public PeersInstance(CoordinatorService service)
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