using Leak.Common;
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

        public bool HasExtensions
        {
            get { return handshake.Options.HasFlag(HandshakeOptions.Extended); }
        }
    }
}