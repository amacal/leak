using Leak.Common;

namespace Leak.Negotiator
{
    public interface HandshakeNegotiatorContext
    {
        void OnHandshake(NetworkConnection negotiated, Handshake handshake);

        void OnDisconnected();
    }
}