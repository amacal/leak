using Leak.Common;
using Leak.Memory;
using System;
using Leak.Networking.Core;
using Leak.Peer.Coordinator;

namespace Leak.Extensions.Peers.Tests
{
    public class PeersSide : IDisposable
    {
        private readonly NetworkConnection connection;
        private readonly Handshake handshake;
        private readonly PeersHooks peers;
        private readonly CoordinatorHooks hooks;

        public PeersSide(NetworkConnection connection, Handshake handshake)
        {
            this.connection = connection;
            this.handshake = handshake;

            this.peers = new PeersHooks();
            this.hooks = new CoordinatorHooks();
        }

        public PeerHash Peer
        {
            get { return handshake.Remote; }
        }

        public Handshake Handshake
        {
            get { return handshake; }
        }

        public PeersInstance Build()
        {
            CoordinatorService service =
                new CoordinatorBuilder()
                    .WithHash(handshake.Hash)
                    .WithMemory(new MemoryBuilder().Build())
                    .WithPlugin(new PeersPlugin(peers))
                    .WithDefinition(new PeersMessages())
                    .Build(hooks);

            return new PeersInstance(service);
        }

        public PeersHooks Peers
        {
            get { return peers; }
        }

        public NetworkConnection Connection
        {
            get { return connection; }
        }

        public CoordinatorHooks Hooks
        {
            get { return hooks; }
        }

        public void Dispose()
        {
        }
    }
}