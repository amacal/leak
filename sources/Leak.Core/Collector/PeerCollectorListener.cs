using Leak.Core.Bouncer;
using Leak.Core.Listener;
using Leak.Core.Loop;
using Leak.Core.Network;

namespace Leak.Core.Collector
{
    public class PeerCollectorListener : PeerListenerCallbackBase
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

        public override void OnConnected(NetworkConnection connection)
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

        public override void OnRejected(NetworkConnection connection)
        {
            callback.OnRejected(connection.Remote);
        }

        public override void OnHandshake(NetworkConnection connection, PeerListenerHandshake handshake)
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
    }
}