using Leak.Core.Core;
using Leak.Sockets;
using Leak.Suckets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace Leak.Core.Network
{
    public class NetworkPool : NetworkPoolListener
    {
        private readonly Dictionary<long, NetworkPoolEntry> items;
        private readonly NetworkPoolConfiguration configuration;
        private readonly NetworkPoolCallback callback;

        private readonly LeakQueue<NetworkPool> queue;
        private readonly TcpSocketFactory factory;
        private readonly CompletionThread worker;

        private long sequence;

        public NetworkPool()
        {
            configuration = new NetworkPoolConfiguration
            {
                Callback = new NetworkPoolCallbackNothing()
            };

            items = new Dictionary<long, NetworkPoolEntry>();
            callback = configuration.Callback;

            queue = new LeakQueue<NetworkPool>(this);
            worker = new CompletionThread();
            factory = new TcpSocketFactory(worker);
        }

        public NetworkPool(Action<NetworkPoolConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Callback = new NetworkPoolCallbackNothing();
            });

            items = new Dictionary<long, NetworkPoolEntry>();
            callback = configuration.Callback;

            queue = new LeakQueue<NetworkPool>(this);
            worker = new CompletionThread();
            factory = new TcpSocketFactory(worker);
        }

        public void Start(LeakPipeline pipeline)
        {
            pipeline.Register(queue);
            worker.Start();
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

            callback.OnAttached(connection);
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

                callback.OnDisconnected(entry.Connection);
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

                callback.OnException(entry.Connection, ex);
            }
        }

        void NetworkPoolListener.Schedule(LeakTask<NetworkPool> task)
        {
            queue.Add(task);
        }
    }
}