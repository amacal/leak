using Leak.Core.Network;
using System;

namespace Leak.Core.Negotiator
{
    public interface HandshakeNegotiatorContext
    {
        void OnHandshake(NetworkConnection connection, Handshake handshake);

        void OnException(Exception ex);

        void OnDisconnected();
    }
}