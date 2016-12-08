using Leak.Completion;
using Leak.Core.Core;
using Leak.Sockets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Leak.Common;

namespace Leak.Core.Network
{
    public class NetworkPool : NetworkPoolListener
    {
        private readonly LeakPipeline pipeline;
        private readonly NetworkPoolHooks hooks;
        private readonly Dictionary<long, NetworkPoolEntry> items;

        private readonly LeakQueue<NetworkPool> queue;
        private readonly TcpSocketFactory factory;

        private long sequence;

        public NetworkPool(LeakPipeline pipeline, CompletionWorker worker, NetworkPoolHooks hooks)
        {
            this.hooks = hooks;
            this.pipeline = pipeline;
            this.factory = new TcpSocketFactory(worker);

            items = new Dictionary<long, NetworkPoolEntry>();
            queue = new LeakQueue<NetworkPool>(this);
        }

        public void Start()
        {
            pipeline.Register(queue);
        }

        public TcpSocket New()
        {
            return factory.Create();
        }

        public NetworkConnection Create(TcpSocket socket, NetworkDirection direction, IPEndPoint remote)
        {
            long identifier = Interlocked.Increment(ref sequence);
            NetworkPoolConnection connection = new NetworkPoolConnection(this, socket, direction, identifier, remote);

            lock (items)
            {
                items.Add(identifier, new NetworkPoolEntry
                {
                    Connection = connection,
                    IsAvailable = true
                });
            }

            hooks.CallConnectionAttached(connection);
            return connection;
        }

        public NetworkConnection Change(NetworkConnection connection, Action<NetworkConnectionConfiguration> configurer)
        {
            NetworkPoolEntry entry;
            long identifier = connection.Identifier;

            lock (items)
            {
                items.TryGetValue(identifier, out entry);
            }

            if (entry?.Connection != null)
            {
                return new NetworkPoolConnection(entry.Connection, configurer);
            }

            throw new InvalidOperationException();
        }

        bool NetworkPoolListener.IsAvailable(long id)
        {
            NetworkPoolEntry entry;

            lock (items)
            {
                items.TryGetValue(id, out entry);
            }

            return entry?.IsAvailable == true;
        }

        public void OnDisconnected(long id)
        {
            NetworkPoolEntry entry;

            lock (items)
            {
                items.TryGetValue(id, out entry);
            }

            if (entry != null)
            {
                entry.IsAvailable = false;
                entry.Connection.Dispose();

                hooks.CallConnectionTerminated(entry.Connection);
            }
        }

        public void OnException(long id, Exception ex)
        {
            NetworkPoolEntry entry;

            lock (items)
            {
                items.TryGetValue(id, out entry);
            }

            if (entry != null)
            {
                entry.IsAvailable = false;
                entry.Connection.Dispose();

                hooks.CallConnectionDropped(entry.Connection, ex.Message);
            }
        }

        void NetworkPoolListener.Schedule(LeakTask<NetworkPool> task)
        {
            queue.Add(task);
        }
    }
}