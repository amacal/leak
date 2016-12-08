using Leak.Events;
using System;

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