using Leak.Core.Events;

namespace Leak.Core.Listener
{
    public static class PeerListenerExtensions
    {
        public static void CallListenerStarted(this PeerListenerHooks hooks, PeerListenerConfiguration configuration)
        {
            hooks.OnListenerStarted?.Invoke(new ListenerStarted
            {
                Peer = configuration.Peer,
                Port = configuration.Port
            });
        }
    }
}