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

        public bool HasExtensions
        {
            get { return handshake.Options.HasFlag(HandshakeOptions.Extended); }
        }
    }
}