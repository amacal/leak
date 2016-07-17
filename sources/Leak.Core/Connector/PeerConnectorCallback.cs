using Leak.Core.Network;
using System;

namespace Leak.Core.Connector
{
    public interface PeerConnectorCallback
    {
        void OnConnected(NetworkConnection connection);

        void OnRejected(NetworkConnection connection);

        void OnHandshake(NetworkConnection connection, PeerConnectorHandshake handshake);

        void OnException(NetworkConnection connection, Exception ex);

        void OnDisconnected(NetworkConnection connection);
    }
}