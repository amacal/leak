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
        private readonly Dictionary<string, PeerCollectorStorageEntry> byEndpoint;

        public PeerCollectorStorage(PeerCollectorConfiguration configuration)
        {
            this.configuration = configuration;
            this.peers = new Dictionary<PeerHash, PeerCollectorStorageEntry>();
            this.byEndpoint = new Dictionary<string, PeerCollectorStorageEntry>();
        }

        public void Add(NetworkConnection connection)
        {
        }

        public void Add(NetworkConnection connection, PeerListenerHandshake handshake)
        {
            peers.Add(handshake.Peer, new PeerCollectorStorageEntry
            {
                Hash = handshake.Hash,
                Peer = handshake.Peer,
                Connection = connection
            });

            byEndpoint.Add(connection.Remote, peers[handshake.Peer]);
        }

        public void Add(NetworkConnection connection, PeerConnectorHandshake handshake)
        {
            peers.Add(handshake.Peer, new PeerCollectorStorageEntry
            {
                Hash = handshake.Hash,
                Peer = handshake.Peer,
                Connection = connection
            });

            byEndpoint.Add(connection.Remote, peers[handshake.Peer]);
        }

        public void Add(ConnectionLoopChannel channel)
        {
            peers[channel.Endpoint.Peer].Channel = channel;
            configuration.Callback.OnConnected(channel.Endpoint);
        }

        public void Remove(PeerHash peer)
        {
            PeerEndpoint endpoint = peers[peer].Channel.Endpoint;

            peers.Remove(peer);
            byEndpoint.Remove(endpoint.Remote);
            configuration.Callback.OnDisconnected(peer);
        }

        public void Remove(NetworkConnection connection)
        {
            string endpoint = connection.Remote;
            PeerCollectorStorageEntry entry;

            if (byEndpoint.TryGetValue(endpoint, out entry))
            {
                peers.Remove(entry.Peer);
                byEndpoint.Remove(connection.Remote);
                configuration.Callback.OnDisconnected(entry.Peer);
            }
        }

        public ConnectionLoopChannel GetChannel(PeerHash peer)
        {
            PeerCollectorStorageEntry entry;
            peers.TryGetValue(peer, out entry);

            return entry?.Channel;
        }
    }
}