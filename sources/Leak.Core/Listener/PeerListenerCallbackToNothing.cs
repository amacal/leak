using Leak.Core.Network;
using System;

namespace Leak.Core.Listener
{
    public class PeerListenerCallbackToNothing : PeerListenerCallback
    {
        public void OnStarted()
        {
        }

        public void OnStopped()
        {
        }

        public void OnConnected(NetworkConnection connection)
        {
        }

        public void OnRejected(NetworkConnection connection)
        {
        }

        public void OnHandshake(NetworkConnection connection, PeerListenerHandshake handshake)
        {
        }

        public void OnException(NetworkConnection connection, Exception ex)
        {
        }

        public void OnDisconnected(NetworkConnection connection)
        {
        }
    }
}