using System;
using Leak.Listener.Events;

namespace Leak.Listener
{
    public class PeerListenerHooks
    {
        public Action<ConnectionArrived> OnConnectionArrived;

        public Action<ListenerStarted> OnListenerStarted;

        public Action<ListenerFailed> OnListenerFailed;
    }
}