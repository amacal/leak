﻿using Leak.Core.Bouncer;
using Leak.Core.Common;
using Leak.Core.Network;
using System;

namespace Leak.Core.Collector
{
    public class PeerCollectorPool : NetworkPoolCallbackBase
    {
        private readonly PeerCollectorCallback callback;
        private readonly PeerBouncer bouncer;
        private readonly PeerCollectorStorage storage;
        private readonly object synchronized;

        public PeerCollectorPool(PeerCollectorCallback callback, PeerBouncer bouncer, PeerCollectorStorage storage, object synchronized)
        {
            this.callback = callback;
            this.bouncer = bouncer;
            this.storage = storage;
            this.synchronized = synchronized;
        }

        public override void OnAttached(NetworkConnection connection)
        {
            lock (synchronized)
            {
                bouncer.AttachConnection(connection);
            }
        }

        public override void OnException(NetworkConnection connection, Exception ex)
        {
            PeerHash peer;

            lock (synchronized)
            {
                bouncer.ReleaseConnection(connection);
                peer = storage.RemoveRemote(PeerAddress.Parse(connection.Remote));
            }

            connection.Terminate();

            if (peer != null)
            {
                callback.OnDisconnected(peer);
            }
        }

        public override void OnDisconnected(NetworkConnection connection)
        {
            PeerHash peer;

            lock (synchronized)
            {
                bouncer.ReleaseConnection(connection);
                peer = storage.RemoveRemote(PeerAddress.Parse(connection.Remote));
            }

            connection.Terminate();

            if (peer != null)
            {
                callback.OnDisconnected(peer);
            }
        }
    }
}