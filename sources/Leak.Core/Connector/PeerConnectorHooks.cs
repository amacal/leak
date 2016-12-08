using Leak.Events;
using System;

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