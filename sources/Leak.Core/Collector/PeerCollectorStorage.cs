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
        private readonly object synchronized;

        private readonly Dictionary<PeerHash, PeerCollectorStorageEntry> byHash;
        private readonly Dictionary<string, PeerCollectorStorageEntry> byEndpoint;

        public PeerCollectorStorage(PeerCollectorConfiguration configuration)
        {
            this.configuration = configuration;
            this.synchronized = new object();

            this.byHash = new Dictionary<PeerHash, PeerCollectorStorageEntry>();
            this.byEndpoint = new Dictionary<string, PeerCollectorStorageEntry>();
        }

        public void Add(NetworkConnection connection)
        {
        }

        public bool Add(NetworkConnection connection, PeerListenerHandshake handshake)
        {
            lock (synchronized)
            {
                if (byHash.ContainsKey(handshake.Peer))
                    return false;

                byHash.Add(handshake.Peer, new PeerCollectorStorageEntry
                {
                    Hash = handshake.Hash,
                    Peer = handshake.Peer,
                    Connection = connection
                });

                byEndpoint.Add(connection.Remote, byHash[handshake.Peer]);
                return true;
            }
        }

        public bool Add(NetworkConnection connection, PeerConnectorHandshake handshake)
        {
            lock (synchronized)
            {
                if (byHash.ContainsKey(handshake.Peer))
                    return false;

                byHash.Add(handshake.Peer, new PeerCollectorStorageEntry
                {
                    Hash = handshake.Hash,
                    Peer = handshake.Peer,
                    Connection = connection
                });

                byEndpoint.Add(connection.Remote, byHash[handshake.Peer]);
                return true;
            }
        }

        public void Add(ConnectionLoopChannel channel)
        {
            lock (synchronized)
            {
                byHash[channel.Endpoint.Peer].Channel = channel;
            }

            configuration.Callback.OnConnected(channel.Endpoint);
        }

        public void Remove(PeerHash peer)
        {
            lock (synchronized)
            {
                PeerCollectorStorageEntry entry;
                byHash.TryGetValue(peer, out entry);

                if (entry != null)
                {
                    PeerEndpoint endpoint = entry.Channel.Endpoint;

                    byHash.Remove(peer);
                    byEndpoint.Remove(endpoint.Remote);
                }
            }

            configuration.Callback.OnDisconnected(peer);
        }

        public void Remove(NetworkConnection connection)
        {
            string endpoint = connection.Remote;
            PeerCollectorStorageEntry entry;

            lock (synchronized)
            {
                if (byEndpoint.TryGetValue(endpoint, out entry))
                {
                    byHash.Remove(entry.Peer);
                    byEndpoint.Remove(connection.Remote);
                }
            }

            if (entry != null)
            {
                configuration.Callback.OnDisconnected(entry.Peer);
            }
        }

        public ConnectionLoopChannel GetChannel(PeerHash peer)
        {
            PeerCollectorStorageEntry entry;

            lock (synchronized)
            {
                byHash.TryGetValue(peer, out entry);
            }

            return entry?.Channel;
        }
    }
}