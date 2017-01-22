using Leak.Events;
using System;

namespace Leak.Listener
{
    public class PeerListenerHooks
    {
        public Action<ConnectionArrived> OnConnectionArrived;

        public Action<ListenerStarted> OnListenerStarted;
    }
}