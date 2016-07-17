using Leak.Core.Network;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Leak.Core.Tests
{
    public class NetworkContainer : IDisposable
    {
        private readonly SocketContainer sockets;
        private readonly Dictionary<string, NetworkConnection> connections;

        public NetworkContainer()
        {
            sockets = new SocketContainer();
            connections = new Dictionary<string, NetworkConnection>();
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
                connections.Add(active, new NetworkConnection(sockets[active], NetworkConnectionDirection.Outgoing));

            if (connections.ContainsKey(passive) == false)
                connections.Add(passive, new NetworkConnection(sockets[passive], NetworkConnectionDirection.Incoming));
        }

        public void ConnectTo(string active, string host, int port)
        {
            sockets.ConnectTo(active, host, port);

            if (connections.ContainsKey(active) == false)
                connections.Add(active, new NetworkConnection(sockets[active], NetworkConnectionDirection.Outgoing));
        }

        public async Task<NetworkConnection> Listen(string passive, int port)
        {
            Socket socket = await sockets.Listen(passive, port);

            if (connections.ContainsKey(passive) == false)
                connections.Add(passive, new NetworkConnection(socket, NetworkConnectionDirection.Incoming));

            return connections[passive];
        }

        public void Dispose()
        {
            sockets.Dispose();
        }
    }
}