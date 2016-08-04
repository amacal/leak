using Leak.Core.Bouncer;
using Leak.Core.Common;
using Leak.Core.Connector;
using Leak.Core.Loop;
using Leak.Core.Network;

namespace Leak.Core.Collector
{
    public class PeerCollectorConnector : PeerConnectorCallbackBase
    {
        private readonly PeerCollectorCallback callback;
        private readonly PeerBouncer bouncer;
        private readonly ConnectionLoop loop;
        private readonly PeerCollectorStorage storage;
        private readonly object synchronized;

        public PeerCollectorConnector(PeerCollectorCallback callback, PeerBouncer bouncer, ConnectionLoop loop, PeerCollectorStorage storage, object synchronized)
        {
            this.callback = callback;
            this.bouncer = bouncer;
            this.loop = loop;
            this.storage = storage;
            this.synchronized = synchronized;
        }

        public override void OnConnecting(PeerConnectorConnecting connecting)
        {
            lock (synchronized)
            {
                if (bouncer.Accept(connecting.Connection) == false)
                {
                    connecting.Reject();
                    return;
                }
            }

            callback.OnConnecting(PeerAddress.Parse(connecting.Connection.Remote));
        }

        public override void OnConnected(NetworkConnection connection)
        {
            int total = 0;
            bool accepted = false;

            lock (synchronized)
            {
                if (bouncer.AcceptRemote(connection))
                {
                    accepted = true;
                    total = bouncer.Count();
                }
            }

            if (accepted)
            {
                PeerAddress peer = PeerAddress.Parse(connection.Remote);
                PeerCollectorConnected connected = new PeerCollectorConnected(peer, total);

                callback.OnConnected(connected);
            }
        }

        public override void OnRejected(NetworkConnection connection)
        {
            callback.OnRejected(PeerAddress.Parse(connection.Remote));
        }

        public override void OnHandshake(NetworkConnection connection, PeerConnectorHandshake handshake)
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