using Leak.Core.Common;
using Leak.Core.Network;
using System;

namespace Leak.Core.Collector
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
            PeerHash peer;

            lock (context.Synchronized)
            {
                context.Bouncer.ReleaseConnection(connection);
                peer = context.Storage.RemoveRemote(PeerAddress.Parse(connection.Remote));
            }

            connection.Terminate();

            if (peer != null)
            {
                context.Callback.OnDisconnected(peer);
            }
        }

        public override void OnDisconnected(NetworkConnection connection)
        {
            PeerHash peer;

            lock (context.Synchronized)
            {
                context.Bouncer.ReleaseConnection(connection);
                peer = context.Storage.RemoveRemote(PeerAddress.Parse(connection.Remote));
            }

            connection.Terminate();

            if (peer != null)
            {
                context.Callback.OnDisconnected(peer);
            }
        }
    }
}