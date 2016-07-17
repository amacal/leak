using Leak.Core.Network;
using System;

namespace Leak.Core.Connector
{
    public class PeerConnectorCallbackToNothing : PeerConnectorCallback
    {
        public void OnConnected(NetworkConnection connection)
        {
        }

        public void OnRejected(NetworkConnection connection)
        {
        }

        public void OnHandshake(NetworkConnection connection, PeerConnectorHandshake handshake)
        {
        }

        public void OnException(NetworkConnection connection, Exception ex)
        {
        }

        public void OnDisconnected(NetworkConnection connection)
        {
        }
    }
}