using Leak.Core.Connector;
using Leak.Core.Loop;
using Leak.Core.Network;
using System;

namespace Leak.Core.Collector
{
    public class PeerCollectorToConnector : PeerConnectorCallback
    {
        private readonly ConnectionLoop loop;
        private readonly PeerCollectorStorage storage;

        public PeerCollectorToConnector(ConnectionLoop loop, PeerCollectorStorage storage)
        {
            this.loop = loop;
            this.storage = storage;
        }

        public void OnConnected(NetworkConnection connection)
        {
            storage.Add(connection);
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