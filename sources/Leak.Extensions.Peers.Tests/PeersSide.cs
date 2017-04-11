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
        private readonly GlueHooks hooks;

        public PeersSide(NetworkConnection connection, Handshake handshake)
        {
            this.connection = connection;
            this.handshake = handshake;

            this.peers = new PeersHooks();
            this.hooks = new GlueHooks();
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
            GlueService service =
                new GlueBuilder()
                    .WithHash(handshake.Hash)
                    .WithMemory(new MemoryBuilder().Build())
                    .WithPlugin(new PeersPlugin(peers))
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

        public GlueHooks Hooks
        {
            get { return hooks; }
        }

        public void Dispose()
        {
        }
    }
}