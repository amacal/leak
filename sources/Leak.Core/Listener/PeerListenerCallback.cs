using Leak.Core.Network;
using System;

namespace Leak.Core.Listener
{
    public interface PeerListenerCallback
    {
        /// <summary>
        /// Called when the listener entered the listening mode.
        /// </summary>
        /// <param name="started">Describes the listening details.</param>
        void OnStarted(PeerListenerStarted started);

        void OnStopped();

        void OnConnecting(PeerListenerConnecting connecting);

        void OnConnected(NetworkConnection connection);

        void OnRejected(NetworkConnection connection);

        void OnHandshake(NetworkConnection connection, PeerListenerHandshake handshake);

        void OnException(NetworkConnection connection, Exception ex);

        void OnDisconnected(NetworkConnection connection);
    }
}