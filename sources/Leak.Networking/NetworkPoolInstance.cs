using Leak.Sockets;
using Leak.Tasks;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Leak.Networking.Core;

namespace Leak.Networking
{
    public class NetworkPoolInstance : NetworkPool
    {
        private readonly NetworkPoolConfiguration configuration;
        private readonly NetworkPoolDependencies dependencies;
        private readonly NetworkPoolHooks hooks;

        private readonly Dictionary<long, NetworkPoolEntry> items;

        private readonly NetworkPoolQueue queue;
        private readonly SocketFactory factory;

        private long sequence;

        public NetworkPoolInstance(NetworkPoolDependencies dependencies, NetworkPoolConfiguration configuration, NetworkPoolHooks hooks)
        {
            this.hooks = hooks;
            this.dependencies = dependencies;
            this.configuration = configuration;

            factory = new SocketFactory(dependencies.Completion);

            items = new Dictionary<long, NetworkPoolEntry>();
            queue = new NetworkPoolQueue(this);
        }

        public void Start()
        {
            dependencies.Pipeline.Register(queue);
        }

        public TcpSocket New()
        {
            return factory.Tcp();
        }

        public NetworkConnection Create(TcpSocket socket, NetworkDirection direction, IPEndPoint remote)
        {
            long identifier = Interlocked.Increment(ref sequence);
            NetworkPoolListener listener = new NetworkPoolListener(items, queue, hooks, configuration, dependencies);
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

            return connection;
        }
    }
}