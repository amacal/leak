using Leak.Core.Common;
using Leak.Core.Connector;
using Leak.Core.Network;

namespace Leak.Core.Collector.Callbacks
{
    public class PeerCollectorToConnector : PeerConnectorCallbackBase
    {
        private readonly PeerCollectorContext context;

        public PeerCollectorToConnector(PeerCollectorContext context)
        {
            this.context = context;
        }

        public override void OnConnecting(PeerConnectorConnecting connecting)
        {
            lock (context.Synchronized)
            {
                if (context.Bouncer.Accept(connecting.Connection) == false)
                {
                    connecting.Reject();
                    return;
                }
            }

            context.Callback.OnConnectingTo(connecting.Hash, PeerAddress.Parse(connecting.Connection.Remote));
        }

        public override void OnConnected(PeerConnectorConnected connected)
        {
            int total = 0;
            bool accepted = false;

            lock (context.Synchronized)
            {
                if (context.Bouncer.AcceptRemote(connected.Connection))
                {
                    accepted = true;
                    total = context.Bouncer.Count();
                }
            }

            if (accepted)
            {
                PeerAddress peer = PeerAddress.Parse(connected.Connection.Remote);
                PeerCollectorConnected payload = new PeerCollectorConnected(peer, total);

                context.Callback.OnConnectedTo(connected.Hash, payload);
            }
            else
            {
                connected.Connection.Terminate();
            }
        }

        public override void OnRejected(NetworkConnection connection)
        {
            context.Callback.OnRejected(PeerAddress.Parse(connection.Remote));
        }

        public override void OnHandshake(NetworkConnection connection, PeerConnectorHandshake handshake)
        {
            PeerAddress address = PeerAddress.Parse(connection.Remote);

            lock (context.Synchronized)
            {
                if (context.Bouncer.AcceptPeer(connection, handshake.Session.Peer))
                {
                    context.Peers.Enlist(handshake.Session, address);
                    context.Cando.Register(handshake);
                }
                else
                {
                    connection.Terminate();
                }
            }

            context.Loop.StartProcessing(connection, handshake);
        }
    }
}