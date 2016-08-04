using Leak.Core.Network;
using System;

namespace Leak.Core.Listener
{
    public interface PeerListenerCallback
    {
        void OnStarted();

        void OnStopped();

        void OnConnecting(PeerListenerConnecting connecting);

        void OnConnected(NetworkConnection connection);

        void OnRejected(NetworkConnection connection);

        void OnHandshake(NetworkConnection connection, PeerListenerHandshake handshake);

        void OnException(NetworkConnection connection, Exception ex);

        void OnDisconnected(NetworkConnection connection);
    }
}