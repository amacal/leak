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
        private readonly object synchronized;

        public PeerCollectorListener(PeerCollectorCallback callback, PeerBouncer bouncer, ConnectionLoop loop, PeerCollectorStorage storage, object synchronized)
        {
            this.callback = callback;
            this.bouncer = bouncer;
            this.loop = loop;
            this.storage = storage;
            this.synchronized = synchronized;
        }

        public void OnStarted()
        {
        }

        public void OnStopped()
        {
        }

        public void OnConnected(NetworkConnection connection)
        {
            bool accepted = false;

            lock (synchronized)
            {
                if (bouncer.AcceptRemote(connection))
                {
                    accepted = true;
                }
            }

            if (accepted)
            {
                callback.OnConnected(connection.Remote);
            }
        }

        public void OnRejected(NetworkConnection connection)
        {
            lock (synchronized)
            {
                bouncer.ReleaseRemote(connection);
            }

            callback.OnRejected(connection.Remote);
        }

        public void OnHandshake(NetworkConnection connection, PeerListenerHandshake handshake)
        {
            lock (synchronized)
            {
                if (bouncer.AcceptPeer(connection, handshake.Peer))
                {
                    storage.AddHandshake(connection, handshake);
                }
            }

            loop.StartProcessing(connection, handshake);
        }

        public void OnException(NetworkConnection connection, Exception ex)
        {
            lock (synchronized)
            {
                bouncer.ReleaseRemote(connection);
                storage.RemoveRemote(connection.Remote);
            }

            connection.Terminate();
        }

        public void OnDisconnected(NetworkConnection connection)
        {
            lock (synchronized)
            {
                bouncer.ReleaseRemote(connection);
                storage.RemoveRemote(connection.Remote);
            }

            connection.Terminate();
        }
    }
}