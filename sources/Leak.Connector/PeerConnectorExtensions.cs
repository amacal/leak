using Leak.Common;
using Leak.Events;
using Leak.Networking.Core;

namespace Leak.Connector
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

        public static void CallConnectionRejected(this PeerConnectorHooks hooks, NetworkAddress remote)
        {
            hooks.OnConnectionRejected?.Invoke(new ConnectionRejected
            {
                Remote = remote
            });
        }
    }
}