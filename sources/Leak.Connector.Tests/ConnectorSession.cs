using System;
using System.Net;
using Leak.Networking;
using Leak.Sockets;

namespace Leak.Connector.Tests
{
    public class ConnectorSession : IDisposable
    {
        private readonly PeerConnector connector;
        private readonly IPEndPoint endpoint;
        private readonly TcpSocket server;

        public ConnectorSession(NetworkPool pool, PeerConnector connector)
        {
            int port;

            this.connector = connector;
            this.server = pool.New();

            server.Bind(out port);
            server.Listen(1);

            endpoint = new IPEndPoint(IPAddress.Loopback, port);
        }

        public IPEndPoint Endpoint
        {
            get { return endpoint; }
        }

        public TcpSocket Server
        {
            get { return server; }
        }

        public PeerConnector Connector
        {
            get { return connector; }
        }

        public void Dispose()
        {
            server?.Dispose();
        }
    }
}
