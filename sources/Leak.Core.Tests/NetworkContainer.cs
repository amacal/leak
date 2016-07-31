using Leak.Core.Network;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Leak.Core.Tests
{
    public class NetworkContainer : IDisposable
    {
        private readonly NetworkPool pool;
        private readonly SocketContainer sockets;
        private readonly Dictionary<string, NetworkConnection> connections;

        public NetworkContainer()
        {
            pool = new NetworkPool();
            sockets = new SocketContainer();
            connections = new Dictionary<string, NetworkConnection>();
        }

        public NetworkPool Pool
        {
            get { return pool; }
        }

        public NetworkConnection this[string name]
        {
            get { return connections[name]; }
        }

        public void Active(string name)
        {
            sockets.Active(name);
        }

        public void Passive(string name)
        {
            sockets.Passive(name);
        }

        public void Connect(string active, string passive)
        {
            sockets.Connect(active, passive);

            if (connections.ContainsKey(active) == false)
                connections.Add(active, pool.Create(sockets[active], NetworkDirection.Outgoing));

            if (connections.ContainsKey(passive) == false)
                connections.Add(passive, pool.Create(sockets[passive], NetworkDirection.Incoming));
        }

        public void ConnectTo(string active, string host, int port)
        {
            sockets.ConnectTo(active, host, port);

            if (connections.ContainsKey(active) == false)
                connections.Add(active, pool.Create(sockets[active], NetworkDirection.Outgoing));
        }

        public async Task<NetworkConnection> Listen(string passive, int port)
        {
            Socket socket = await sockets.Listen(passive, port);

            if (connections.ContainsKey(passive) == false)
                connections.Add(passive, pool.Create(socket, NetworkDirection.Incoming));

            return connections[passive];
        }

        public void Dispose()
        {
            sockets.Dispose();
        }
    }
}