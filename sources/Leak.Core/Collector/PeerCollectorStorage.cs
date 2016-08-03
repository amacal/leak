using Leak.Core.Common;
using Leak.Core.Connector;
using Leak.Core.Listener;
using Leak.Core.Loop;
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

            entry.Remote = address;
            entry.Peer = handshake.Peer;
            entry.Hash = handshake.Hash;
            entry.HasExtensions = handshake.HasExtensions;

            collection.AddByPeer(handshake.Peer, entry);
            collection.AddByHash(handshake.Hash, entry);
        }

        public void AddHandshake(NetworkConnection connection, PeerConnectorHandshake handshake)
        {
            PeerAddress address = PeerAddress.Parse(connection.Remote);
            PeerCollectorStorageEntry entry = collection.CreateByRemote(address);

            entry.Remote = address;
            entry.Peer = handshake.Peer;
            entry.Hash = handshake.Hash;
            entry.HasExtensions = handshake.HasExtensions;

            collection.AddByPeer(handshake.Peer, entry);
            collection.AddByHash(handshake.Hash, entry);
        }

        public void AttachChannel(ConnectionLoopChannel channel)
        {
            PeerCollectorStorageEntry entry = collection.FindByRemote(channel.Endpoint.Remote);

            if (entry != null)
            {
                entry.Loop = channel;
            }
        }

        public void RemoveRemote(PeerAddress remote)
        {
            PeerCollectorStorageEntry entry = collection.FindByRemote(remote);

            if (entry != null)
            {
                collection.RemoveByRemote(entry.Remote);
                collection.RemoveByPeer(entry.Peer);
            }
        }

        public bool IsInterested(PeerHash peer)
        {
            return collection.FindByPeer(peer)?.LocalState.IsInterested() == true;
        }

        public void SetInterested(PeerHash peer, bool value)
        {
            collection.FindByPeer(peer)?.LocalState.SetInterested(value);
        }

        public bool IsChoked(PeerHash peer)
        {
            return collection.FindByPeer(peer)?.RemoteState.IsChoked() == true;
        }

        public void SetChoked(PeerHash peer, bool value)
        {
            collection.FindByPeer(peer)?.RemoteState.SetChoked(value);
        }

        public PeerCollectorChannel GetChannel(PeerHash peer)
        {
            return new PeerCollectorChannel(configuration.Callback, collection.FindByPeer(peer)?.Loop);
        }

        public bool SupportsExtensions(PeerHash peer)
        {
            return collection.FindByPeer(peer)?.HasExtensions == true;
        }
    }
}