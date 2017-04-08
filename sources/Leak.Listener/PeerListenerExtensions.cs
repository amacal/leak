using Leak.Common;
using Leak.Listener.Events;
using Leak.Networking.Core;

namespace Leak.Listener
{
    public static class PeerListenerExtensions
    {
        public static void CallListenerFailed(this PeerListenerHooks hooks, PeerListenerConfiguration configuration, string reason)
        {
            hooks.OnListenerFailed?.Invoke(new ListenerFailed
            {
                Peer = configuration.Peer,
                Reason = reason
            });
        }

        public static void CallListenerStarted(this PeerListenerHooks hooks, PeerListenerConfiguration configuration, int assignedPort)
        {
            hooks.OnListenerStarted?.Invoke(new ListenerStarted
            {
                Peer = configuration.Peer,
                Port = assignedPort
            });
        }

        public static void CallConnectionArrived(this PeerListenerHooks hooks, NetworkAddress remote, NetworkConnection connection)
        {
            hooks.OnConnectionArrived?.Invoke(new ConnectionArrived
            {
                Remote = remote,
                Connection = connection
            });
        }
    }
}