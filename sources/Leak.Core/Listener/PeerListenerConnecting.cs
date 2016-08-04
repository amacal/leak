using Leak.Core.Network;
using System;

namespace Leak.Core.Listener
{
    public class PeerListenerConnecting
    {
        private readonly NetworkConnectionInfo connection;
        private readonly Action onRejected;

        public PeerListenerConnecting(NetworkConnectionInfo connection, Action onRejected)
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