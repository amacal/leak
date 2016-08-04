using Leak.Core.Network;
using System;

namespace Leak.Core.Connector
{
    public class PeerConnectorConnecting
    {
        private readonly NetworkConnectionInfo connection;
        private readonly Action onRejected;

        public PeerConnectorConnecting(NetworkConnectionInfo connection, Action onRejected)
        {
            this.connection = connection;
            this.onRejected = onRejected;
        }

        public NetworkConnectionInfo Connection
        {
            get { return connection; }
        }

        public void Reject()
        {
            onRejected.Invoke();
        }
    }
}