using Leak.Common;
using Leak.Core.Events;
using Leak.Core.Negotiator;
using Leak.Core.Network;
using Leak.Events;

namespace Leak.Core.Listener
{
    public static class PeerListenerExtensions
    {
        public static void CallListenerStarted(this PeerListenerHooks hooks, PeerListenerConfiguration configuration, int assignedPort)
        {
            hooks.OnListenerStarted?.Invoke(new ListenerStarted
            {
                Peer = configuration.Peer,
                Port = assignedPort
            });
        }

        public static void CallConnectionArrived(this PeerListenerHooks hooks, PeerAddress remote)
        {
            hooks.OnConnectionArrived?.Invoke(new ConnectionArrived
            {
                Remote = remote
            });
        }

        public static void CallHandshakeCompleted(this PeerListenerHooks hooks, NetworkConnection connection, Handshake handshake)
        {
            hooks.OnHandshakeCompleted?.Invoke(new HandshakeCompleted
            {
                Connection = connection,
                Handshake = handshake
            });
        }

        public static void CallHandshakeRejected(this PeerListenerHooks hooks, NetworkConnection connection)
        {
            hooks.OnHandshakeRejected?.Invoke(new HandshakeRejected
            {
                Connection = connection
            });
        }
    }
}