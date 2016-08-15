using Leak.Core.Common;
using Leak.Core.Connector;
using Leak.Core.Listener;
using Leak.Core.Network;

namespace Leak.Core.Collector
{
    public class PeerCollectorStorage
    {
        private readonly PeerCollectorConfiguration configuration;
        private readonly PeerCollectorStorageEntryCollection collection;

        public PeerCollectorStorage(PeerCollectorConfiguration configuration)
        {
            this.configuration = configuration;
            this.collection = new PeerCollectorStorageEntryCollection();
        }

        public void AddHandshake(NetworkConnection connection, PeerListenerHandshake handshake)
        {
            PeerAddress address = PeerAddress.Parse(connection.Remote);
            PeerCollectorStorageEntry entry = collection.CreateByRemote(address);

            entry.Endpoint = handshake.ToEndpoint(address);
            entry.HasExtensions = handshake.HasExtensions;

            collection.AddByPeer(handshake.Peer, entry);
            collection.AddByHash(handshake.Hash, entry);
        }

        public void AddHandshake(NetworkConnection connection, PeerConnectorHandshake handshake)
        {
            PeerAddress address = PeerAddress.Parse(connection.Remote);
            PeerCollectorStorageEntry entry = collection.CreateByRemote(address);

            entry.Endpoint = handshake.ToEndpoint(address);
            entry.HasExtensions = handshake.HasExtensions;

            collection.AddByPeer(handshake.Peer, entry);
            collection.AddByHash(handshake.Hash, entry);
        }

        public PeerHash RemoveRemote(PeerAddress remote)
        {
            PeerCollectorStorageEntry entry = collection.FindByRemote(remote);

            if (entry != null)
            {
                collection.RemoveByRemote(entry.Endpoint.Remote);
                collection.RemoveByPeer(entry.Endpoint.Peer);
            }

            return entry?.Endpoint.Peer;
        }
    }
}