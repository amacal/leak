using Leak.Common;
using Leak.Completion;
using Leak.Sockets;
using Leak.Tasks;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace Leak.Networking
{
    public class NetworkPool
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
            NetworkPoolListener listener = new NetworkPoolListener(items, queue, hooks);
            NetworkPoolConnection connection = new NetworkPoolConnection(listener, socket, direction, identifier, remote);

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
    }
}