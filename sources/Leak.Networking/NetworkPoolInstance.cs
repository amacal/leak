using Leak.Common;
using Leak.Sockets;
using Leak.Tasks;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace Leak.Networking
{
    public class NetworkPoolInstance : NetworkPool
    {
        private readonly LeakPipeline pipeline;
        private readonly NetworkPoolHooks hooks;
        private readonly Dictionary<long, NetworkPoolEntry> items;

        private readonly LeakQueue<NetworkPoolInstance> queue;
        private readonly SocketFactory factory;

        private long sequence;

        public NetworkPoolInstance(NetworkPoolDependencies dependencies, NetworkPoolHooks hooks)
        {
            this.hooks = hooks;

            pipeline = dependencies.Pipeline;
            factory = new SocketFactory(dependencies.Completion);

            items = new Dictionary<long, NetworkPoolEntry>();
            queue = new LeakQueue<NetworkPoolInstance>(this);
        }

        public void Start()
        {
            pipeline.Register(queue);
        }

        public TcpSocket New()
        {
            return factory.Tcp();
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

        public NetworkConnection Change(NetworkConnection connection, NetworkConfiguration configurer)
        {
            NetworkPoolEntry entry;
            long identifier = connection.Identifier;

            lock (items)
            {
                items.TryGetValue(identifier, out entry);
            }

            if (entry?.Connection != null)
            {
                connection = new NetworkPoolConnection(entry.Connection, configurer);
                hooks.CallConnectionEncrypted(connection);
                return connection;
            }

            throw new InvalidOperationException();
        }
    }
}