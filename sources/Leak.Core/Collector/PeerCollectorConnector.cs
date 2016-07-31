using Leak.Core.Connector;
using Leak.Core.Loop;
using Leak.Core.Network;
using System;

namespace Leak.Core.Collector
{
    public class PeerCollectorConnector : PeerConnectorCallback
    {
        private readonly PeerCollectorCallback callback;
        private readonly ConnectionLoop loop;
        private readonly PeerCollectorStorage storage;

        public PeerCollectorConnector(PeerCollectorCallback callback, ConnectionLoop loop, PeerCollectorStorage storage)
        {
            this.callback = callback;
            this.loop = loop;
            this.storage = storage;
        }

        public void OnConnected(NetworkConnection connection)
        {
            callback.OnConnected(connection.Remote);
        }

        public void OnRejected(NetworkConnection connection)
        {
        }

        public void OnHandshake(NetworkConnection connection, PeerConnectorHandshake handshake)
        {
            if (storage.Add(connection, handshake))
            {
                loop.Handle(connection, handshake);
            }
        }

        public void OnException(NetworkConnection connection, Exception ex)
        {
            storage.Remove(connection);
        }

        public void OnDisconnected(NetworkConnection connection)
        {
            storage.Remove(connection);
        }
    }
}