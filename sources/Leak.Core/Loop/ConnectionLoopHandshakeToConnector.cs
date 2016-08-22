using Leak.Core.Common;
using Leak.Core.Connector;

namespace Leak.Core.Loop
{
    public class ConnectionLoopHandshakeToConnector : ConnectionLoopHandshake
    {
        private readonly PeerConnectorHandshake handshake;

        public ConnectionLoopHandshakeToConnector(PeerConnectorHandshake handshake)
        {
            this.handshake = handshake;
        }

        public PeerSession Session
        {
            get { return handshake.Session; }
        }

        public bool HasExtensions
        {
            get { return handshake.HasExtensions; }
        }
    }
}