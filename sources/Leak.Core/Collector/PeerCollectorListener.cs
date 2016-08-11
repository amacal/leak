using Leak.Core.Common;
using Leak.Core.Listener;
using Leak.Core.Network;

namespace Leak.Core.Collector
{
    public class PeerCollectorListener : PeerListenerCallbackBase
    {
        private readonly PeerCollectorContext context;

        public PeerCollectorListener(PeerCollectorContext context)
        {
            this.context = context;
        }

        public override void OnConnecting(PeerListenerConnecting connecting)
        {
            lock (context.Synchronized)
            {
                if (context.Bouncer.Accept(connecting.Connection) == false)
                {
                    connecting.Reject();
                    return;
                }
            }

            context.Callback.OnConnecting(PeerAddress.Parse(connecting.Connection.Remote));
        }

        public override void OnConnected(NetworkConnection connection)
        {
            int total = 0;
            bool accepted = false;

            lock (context.Synchronized)
            {
                if (context.Bouncer.AcceptRemote(connection))
                {
                    accepted = true;
                    total = context.Bouncer.Count();
                }
            }

            if (accepted)
            {
                PeerAddress peer = PeerAddress.Parse(connection.Remote);
                PeerCollectorConnected connected = new PeerCollectorConnected(peer, total);

                context.Callback.OnConnected(connected);
            }
            else
            {
                connection.Terminate();
            }
        }

        public override void OnRejected(NetworkConnection connection)
        {
            context.Callback.OnRejected(PeerAddress.Parse(connection.Remote));
        }

        public override void OnHandshake(NetworkConnection connection, PeerListenerHandshake handshake)
        {
            lock (context.Synchronized)
            {
                if (context.Bouncer.AcceptPeer(connection, handshake.Peer))
                {
                    context.Peers.Enlist(handshake.Peer, handshake.Hash);
                    context.Storage.AddHandshake(connection, handshake);
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