using Leak.Core.Common;
using Leak.Core.Listener;

namespace Leak.Core.Loop
{
    public class ConnectionLoopHandshakeToListener : ConnectionLoopHandshake
    {
        private readonly PeerListenerHandshake handshake;

        public ConnectionLoopHandshakeToListener(PeerListenerHandshake handshake)
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
    }
}