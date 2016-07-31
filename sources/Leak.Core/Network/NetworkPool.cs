using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace Leak.Core.Network
{
    public class NetworkPool
    {
        private long sequence;
        private readonly Dictionary<long, NetworkConnectionSocket> items;

        public NetworkPool()
        {
            items = new Dictionary<long, NetworkConnectionSocket>();
        }

        public NetworkConnection Create(Socket socket, NetworkDirection direction)
        {
            long identifier = Interlocked.Increment(ref sequence);
            NetworkConnectionSocket connection = new NetworkConnectionSocket(socket, direction, identifier);

            lock (items)
            {
                items.Add(identifier, connection);
            }

            return connection;
        }

        public NetworkConnection Change(NetworkConnection connection, Action<NetworkConnectionConfiguration> configurer)
        {
            NetworkConnectionSocket source;
            long identifier = connection.Identifier;

            lock (items)
            {
                items.TryGetValue(identifier, out source);
            }

            if (source != null)
            {
                return new NetworkConnectionSocket(source, configurer);
            }

            throw new InvalidOperationException();
        }
    }
}