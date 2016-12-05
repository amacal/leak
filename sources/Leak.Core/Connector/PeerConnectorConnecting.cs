using Leak.Core.Network;
using System;
using Leak.Common;

namespace Leak.Core.Connector
{
    public class PeerConnectorConnecting
    {
        private readonly FileHash hash;
        private readonly NetworkConnectionInfo connection;
        private readonly Action onRejected;

        public PeerConnectorConnecting(FileHash hash, NetworkConnectionInfo connection, Action onRejected)
        {
            this.hash = hash;
            this.connection = connection;
            this.onRejected = onRejected;
        }

        public FileHash Hash
        {
            get { return hash; }
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