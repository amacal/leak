using Leak.Core.Common;
using Leak.Core.Negotiator;

namespace Leak.Core.Listener
{
    public class PeerListenerHandshake
    {
        private readonly Handshake handshake;

        public PeerListenerHandshake(Handshake handshake)
        {
            this.handshake = handshake;
        }

        public PeerHash Peer
        {
            get { return handshake.Remote; }
        }

        public FileHash Hash
        {
            get { return handshake.Hash; }
        }
    }
}