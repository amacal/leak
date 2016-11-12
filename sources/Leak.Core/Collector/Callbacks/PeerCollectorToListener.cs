using Leak.Core.Common;
using Leak.Core.Listener;
using Leak.Core.Network;

namespace Leak.Core.Collector.Callbacks
{
    public class PeerCollectorToListener : PeerListenerCallbackBase
    {
        private readonly PeerCollectorContext context;

        public PeerCollectorToListener(PeerCollectorContext context)
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
                }
            }
        }

        public override void OnRejected(NetworkConnection connection)
        {
            context.Callback.OnRejected(connection.Remote);
        }

        public override void OnHandshake(NetworkConnection connection, PeerListenerHandshake handshake)
        {
            PeerAddress address = connection.Remote;

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
                    return;
                }
            }

            context.Loop.StartProcessing(connection, handshake);
        }
    }
}