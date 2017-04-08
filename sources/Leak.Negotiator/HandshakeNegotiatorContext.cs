using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Negotiator
{
    public interface HandshakeNegotiatorContext
    {
        void OnHandshake(NetworkConnection negotiated, Handshake handshake);

        void OnDisconnected();
    }
}