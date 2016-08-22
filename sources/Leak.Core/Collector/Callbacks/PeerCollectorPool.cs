using Leak.Core.Common;
using Leak.Core.Network;
using System;

namespace Leak.Core.Collector.Callbacks
{
    public class PeerCollectorPool : NetworkPoolCallbackBase
    {
        private readonly PeerCollectorContext context;

        public PeerCollectorPool(PeerCollectorContext context)
        {
            this.context = context;
        }

        public override void OnAttached(NetworkConnection connection)
        {
            lock (context.Synchronized)
            {
                context.Bouncer.AttachConnection(connection);
            }
        }

        public override void OnException(NetworkConnection connection, Exception ex)
        {
            PeerSession session;
            PeerAddress address = PeerAddress.Parse(connection.Remote);

            lock (context.Synchronized)
            {
                context.Bouncer.ReleaseConnection(connection);
                session = context.Peers.Dismiss(address);
            }

            connection.Terminate();

            if (session != null)
            {
                context.Callback.OnDisconnected(session);
            }
        }

        public override void OnDisconnected(NetworkConnection connection)
        {
            PeerSession session;
            PeerAddress address = PeerAddress.Parse(connection.Remote);

            lock (context.Synchronized)
            {
                context.Bouncer.ReleaseConnection(connection);
                session = context.Peers.Dismiss(address);
            }

            connection.Terminate();

            if (session != null)
            {
                context.Callback.OnDisconnected(session);
            }
        }
    }
}