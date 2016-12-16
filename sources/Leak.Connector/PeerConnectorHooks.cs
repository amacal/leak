using System;
using Leak.Events;

namespace Leak.Connector
{
    public class PeerConnectorHooks
    {
        public Action<ConnectionEstablished> OnConnectionEstablished;

        public Action<ConnectionRejected> OnConnectionRejected;
    }
}