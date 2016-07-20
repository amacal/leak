using Leak.Core.Common;
using Leak.Core.Connector;
using Leak.Core.Listener;
using Leak.Core.Loop;
using Leak.Core.Network;
using System.Collections.Generic;

namespace Leak.Core.Collector
{
    public class PeerCollectorStorage
    {
        private readonly PeerCollectorConfiguration configuration;
        private readonly Dictionary<PeerHash, PeerCollectorStorageEntry> peers;

        public PeerCollectorStorage(PeerCollectorConfiguration configuration)
        {
            this.configuration = configuration;
            this.peers = new Dictionary<PeerHash, PeerCollectorStorageEntry>();
        }

        public void Add(NetworkConnection connection, PeerListenerHandshake handshake)
        {
            peers.Add(handshake.Peer, new PeerCollectorStorageEntry
            {
                Hash = handshake.Hash,
                Peer = handshake.Peer,
                Connection = connection
            });
        }

        public void Add(NetworkConnection connection, PeerConnectorHandshake handshake)
        {
            peers.Add(handshake.Peer, new PeerCollectorStorageEntry
            {
                Hash = handshake.Hash,
                Peer = handshake.Peer,
                Connection = connection
            });
        }

        public void Add(ConnectionLoopChannel channel)
        {
            peers[channel.Peer].Channel = channel;
            configuration.Callback.OnConnected(channel.Peer, channel.Hash);
        }

        public void Remove(PeerHash peer)
        {
            peers.Remove(peer);
        }

        public void Remove(NetworkConnection connection)
        {
        }

        public ConnectionLoopChannel GetChannel(PeerHash peer)
        {
            return peers[peer].Channel;
        }
    }
}