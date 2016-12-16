using Leak.Common;

namespace Leak.Connector
{
    public class PeerConnectorHandshake
    {
        private readonly Handshake handshake;

        public PeerConnectorHandshake(Handshake handshake)
        {
            this.handshake = handshake;
        }

        public bool HasExtensions
        {
            get { return handshake.Options.HasFlag(HandshakeOptions.Extended); }
        }
    }
}