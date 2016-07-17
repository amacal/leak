using Leak.Core.Common;
using Leak.Core.Negotiator;

namespace Leak.Core.Connector
{
    public class PeerConnectorHandshake
    {
        private readonly Handshake handshake;

        public PeerConnectorHandshake(Handshake handshake)
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