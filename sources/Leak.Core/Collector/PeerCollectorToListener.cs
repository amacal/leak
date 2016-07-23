using Leak.Core.Listener;
using Leak.Core.Loop;
using Leak.Core.Network;
using System;

namespace Leak.Core.Collector
{
    public class PeerCollectorToListener : PeerListenerCallback
    {
        private readonly ConnectionLoop loop;
        private readonly PeerCollectorStorage storage;

        public PeerCollectorToListener(ConnectionLoop loop, PeerCollectorStorage storage)
        {
            this.loop = loop;
            this.storage = storage;
        }

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
            if (storage.Add(connection, handshake))
            {
                loop.Handle(connection, handshake);
            }
        }

        public void OnException(NetworkConnection connection, Exception ex)
        {
            storage.Remove(connection);
        }

        public void OnDisconnected(NetworkConnection connection)
        {
            storage.Remove(connection);
        }
    }
}