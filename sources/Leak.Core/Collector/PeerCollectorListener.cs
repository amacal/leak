using Leak.Core.Bouncer;
using Leak.Core.Listener;
using Leak.Core.Loop;
using Leak.Core.Network;
using System;

namespace Leak.Core.Collector
{
    public class PeerCollectorListener : PeerListenerCallback
    {
        private readonly PeerCollectorCallback callback;
        private readonly PeerBouncer bouncer;
        private readonly ConnectionLoop loop;
        private readonly PeerCollectorStorage storage;

        public PeerCollectorListener(PeerCollectorCallback callback, PeerBouncer bouncer, ConnectionLoop loop, PeerCollectorStorage storage)
        {
            this.callback = callback;
            this.bouncer = bouncer;
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
            if (bouncer.AcceptRemote(connection))
            {
                callback.OnConnected(connection.Remote);
            }
        }

        public void OnRejected(NetworkConnection connection)
        {
            bouncer.ReleaseRemote(connection);
            callback.OnRejected(connection.Remote);
        }

        public void OnHandshake(NetworkConnection connection, PeerListenerHandshake handshake)
        {
            if (bouncer.AcceptPeer(connection, handshake.Peer))
            {
                storage.Add(connection, handshake);
                loop.Handle(connection, handshake);
            }
        }

        public void OnException(NetworkConnection connection, Exception ex)
        {
            bouncer.ReleaseRemote(connection);
            storage.Remove(connection);
        }

        public void OnDisconnected(NetworkConnection connection)
        {
            bouncer.ReleaseRemote(connection);
            storage.Remove(connection);
        }
    }
}