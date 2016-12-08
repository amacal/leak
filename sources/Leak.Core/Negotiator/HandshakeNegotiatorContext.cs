using Leak.Common;
using Leak.Core.Network;

namespace Leak.Core.Negotiator
{
    public interface HandshakeNegotiatorContext
    {
        void OnHandshake(NetworkConnection negotiated, Handshake handshake);

        void OnDisconnected();
    }
}