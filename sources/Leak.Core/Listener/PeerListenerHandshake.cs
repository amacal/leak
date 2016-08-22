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

        public PeerSession Session
        {
            get { return new PeerSession(handshake.Hash, handshake.Remote); }
        }

        public bool HasExtensions
        {
            get { return handshake.Options.HasFlag(HandshakeOptions.Extended); }
        }
    }
}