using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace Leak.Core.Network
{
    public class NetworkPool : NetworkPoolListener
    {
        private long sequence;
        private readonly Dictionary<long, NetworkPoolEntry> items;

        public NetworkPool()
        {
            items = new Dictionary<long, NetworkPoolEntry>();
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

        void NetworkPoolListener.OnDisconnected(long id)
        {
            NetworkPoolEntry entry;

            lock (items)
            {
                items.TryGetValue(id, out entry);
            }

            if (entry != null)
            {
                entry.IsAvailable = false;
            }
        }

        void NetworkPoolListener.OnException(long id, Exception ex)
        {
            NetworkPoolEntry entry;

            lock (items)
            {
                items.TryGetValue(id, out entry);
            }

            if (entry != null)
            {
                entry.IsAvailable = false;
            }
        }
    }
}