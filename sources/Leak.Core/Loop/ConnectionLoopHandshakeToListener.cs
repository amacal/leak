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