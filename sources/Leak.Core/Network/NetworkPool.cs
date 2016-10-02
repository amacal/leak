using Leak.Core.Core;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace Leak.Core.Network
{
    public class NetworkPool : NetworkPoolListener
    {
        private readonly Dictionary<long, NetworkPoolEntry> items;
        private readonly NetworkPoolConfiguration configuration;
        private readonly NetworkPoolCallback callback;

        private readonly LeakTimer timer;
        private readonly LeakQueue<NetworkPool> queue;

        private long sequence;

        public NetworkPool()
        {
            configuration = new NetworkPoolConfiguration
            {
                Callback = new NetworkPoolCallbackNothing()
            };

            items = new Dictionary<long, NetworkPoolEntry>();
            callback = configuration.Callback;

            queue = new LeakQueue<NetworkPool>();
            timer = new LeakTimer(TimeSpan.FromMilliseconds(10));
        }

        public NetworkPool(Action<NetworkPoolConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Callback = new NetworkPoolCallbackNothing();
            });

            items = new Dictionary<long, NetworkPoolEntry>();
            callback = configuration.Callback;

            queue = new LeakQueue<NetworkPool>();
            timer = new LeakTimer(TimeSpan.FromMilliseconds(50));
        }

        public void Start()
        {
            queue.Start(this);
        }

        public NetworkConnectionInfo Info(string remote, NetworkDirection direction)
        {
            return new NetworkConnectionInfo(remote, direction);
        }

        public NetworkConnectionInfo Info(Socket socket, NetworkDirection direction)
        {
            string remote = NetworkPoolConnection.GetRemote(socket);
            NetworkConnectionInfo info = new NetworkConnectionInfo(remote, direction);

            return info;
        }

        public NetworkConnection Create(Socket socket, NetworkDirection direction)
        {
            long identifier = Interlocked.Increment(ref sequence);
            NetworkPoolConnection connection = new NetworkPoolConnection(this, socket, direction, identifier);

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
                callback.OnException(entry.Connection, ex);
            }
        }

        void NetworkPoolListener.Schedule(LeakTask<NetworkPool> task)
        {
            queue.Add(task);
        }
    }
}