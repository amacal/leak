using System;
using System.Net;
using Leak.Networking;
using Leak.Sockets;

namespace Leak.Listener.Tests
{
    public class ListenerSession : IDisposable
    {
        private readonly IPEndPoint endpoint;
        private readonly TcpSocket client;

        public ListenerSession(int port, NetworkPool pool)
        {
            endpoint = new IPEndPoint(IPAddress.Loopback, port);

            client = pool.New();
            client.Bind();
        }

        public IPEndPoint Endpoint
        {
            get { return endpoint; }
        }

        public TcpSocket Client
        {
            get { return client; }
        }

        public void Dispose()
        {
            client?.Dispose();
        }
    }
}
