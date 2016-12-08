using Leak.Common;

namespace Leak.Core.Negotiator
{
    public interface HandshakeNegotiatorContext
    {
        void OnHandshake(NetworkConnection negotiated, Handshake handshake);

        void OnDisconnected();
    }
}