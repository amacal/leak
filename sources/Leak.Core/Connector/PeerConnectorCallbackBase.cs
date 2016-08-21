using Leak.Core.Network;
using System;

namespace Leak.Core.Connector
{
    public abstract class PeerConnectorCallbackBase : PeerConnectorCallback
    {
        public virtual void OnConnecting(PeerConnectorConnecting connecting)
        {
        }

        public virtual void OnConnected(PeerConnectorConnected connected)
        {
        }

        public virtual void OnRejected(NetworkConnection connection)
        {
        }

        public virtual void OnHandshake(NetworkConnection connection, PeerConnectorHandshake handshake)
        {
        }

        public virtual void OnException(NetworkConnection connection, Exception ex)
        {
        }

        public virtual void OnDisconnected(NetworkConnection connection)
        {
        }
    }
}