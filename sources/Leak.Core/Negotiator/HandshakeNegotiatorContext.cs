using Leak.Core.Network;
using System;

namespace Leak.Core.Negotiator
{
    public interface HandshakeNegotiatorContext
    {
        void OnHandshake(NetworkConnection negotiated, Handshake handshake);

        void OnException(Exception ex);

        void OnDisconnected();
    }
}