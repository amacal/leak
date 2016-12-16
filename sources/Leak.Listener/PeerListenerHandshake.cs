using Leak.Common;

namespace Leak.Listener
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