using Leak.Core.Network;
using System;

namespace Leak.Core.Listener
{
    public interface PeerListenerCallback
    {
        void OnStopped();

        void OnConnecting(PeerListenerConnecting connecting);

        void OnRejected(NetworkConnection connection);

        void OnHandshake(NetworkConnection connection, PeerListenerHandshake handshake);

        void OnException(NetworkConnection connection, Exception ex);

        void OnDisconnected(NetworkConnection connection);
    }
}