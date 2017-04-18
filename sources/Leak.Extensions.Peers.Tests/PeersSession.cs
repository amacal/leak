using System;
using Leak.Peer.Coordinator;

namespace Leak.Extensions.Peers.Tests
{
    public class PeersSession : IDisposable
    {
        private readonly CoordinatorService coordinator;
        private readonly PeersPlugin plugin;

        public PeersSession(CoordinatorService coordinator, PeersPlugin plugin)
        {
            this.coordinator = coordinator;
            this.plugin = plugin;
        }

        public CoordinatorService Coordinator
        {
            get { return coordinator; }
        }

        public PeersPlugin Plugin
        {
            get { return plugin; }
        }

        public void Dispose()
        {
        }
    }
}