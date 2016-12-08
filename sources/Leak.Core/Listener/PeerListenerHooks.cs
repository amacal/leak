using Leak.Core.Events;
using System;
using Leak.Events;

namespace Leak.Core.Listener
{
    public class PeerListenerHooks
    {
        public Action<ConnectionArrived> OnConnectionArrived;

        public Action<HandshakeCompleted> OnHandshakeCompleted;

        public Action<HandshakeRejected> OnHandshakeRejected;

        public Action<ListenerStarted> OnListenerStarted;
    }
}