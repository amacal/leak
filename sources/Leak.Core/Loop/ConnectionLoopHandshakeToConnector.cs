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

        public PeerHash Peer
        {
            get { return handshake.Peer; }
        }

        public FileHash Hash
        {
            get { return handshake.Hash; }
        }

        public bool HasExtensions
        {
            get { return handshake.HasExtensions; }
        }
    }
}