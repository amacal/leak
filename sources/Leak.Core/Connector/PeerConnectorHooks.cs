using Leak.Core.Events;
using System;
using Leak.Events;

namespace Leak.Core.Connector
{
    public class PeerConnectorHooks
    {
        public Action<ConnectionEstablished> OnConnectionEstablished;

        public Action<ConnectionRejected> OnConnectionRejected;

        public Action<HandshakeCompleted> OnHandshakeCompleted;

        public Action<HandshakeRejected> OnHandshakeRejected;
    }
}