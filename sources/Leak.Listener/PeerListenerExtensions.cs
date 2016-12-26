using Leak.Common;
using Leak.Events;

namespace Leak.Listener
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

        public static void CallConnectionArrived(this PeerListenerHooks hooks, PeerAddress remote, NetworkConnection connection)
        {
            hooks.OnConnectionArrived?.Invoke(new ConnectionArrived
            {
                Remote = remote,
                Connection = connection
            });
        }
    }
}