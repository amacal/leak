using Leak.Common;
using Leak.Core.Negotiator;
using Leak.Events;

namespace Leak.Core.Connector
{
    public static class PeerConnectorExtensions
    {
        public static void CallConnectionEstablished(this PeerConnectorHooks hooks, NetworkConnection connection)
        {
            hooks.OnConnectionEstablished?.Invoke(new ConnectionEstablished
            {
                Remote = connection.Remote,
                Connection = connection
            });
        }

        public static void CallConnectionRejected(this PeerConnectorHooks hooks, PeerAddress remote)
        {
            hooks.OnConnectionRejected?.Invoke(new ConnectionRejected
            {
                Remote = remote
            });
        }

        public static void CallHandshakeCompleted(this PeerConnectorHooks hooks, NetworkConnection connection, Handshake handshake)
        {
            hooks.OnHandshakeCompleted?.Invoke(new HandshakeCompleted
            {
                Connection = connection,
                Handshake = handshake
            });
        }

        public static void CallHandshakeRejected(this PeerConnectorHooks hooks, NetworkConnection connection)
        {
            hooks.OnHandshakeRejected?.Invoke(new HandshakeRejected
            {
                Connection = connection
            });
        }
    }
}