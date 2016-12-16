using System;
using Leak.Events;

namespace Leak.Listener
{
    public class PeerListenerHooks
    {
        public Action<ConnectionArrived> OnConnectionArrived;

        public Action<ListenerStarted> OnListenerStarted;
    }
}