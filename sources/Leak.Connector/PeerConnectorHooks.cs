using Leak.Events;
using System;

namespace Leak.Connector
{
    public class PeerConnectorHooks
    {
        public Action<ConnectionEstablished> OnConnectionEstablished;

        public Action<ConnectionRejected> OnConnectionRejected;
    }
}