using Leak.Sockets;
using System;

namespace Leak.Loop.Tests
{
    public class LoopSession : IDisposable
    {
        private readonly TcpSocket client;
        private readonly ConnectionLoop loop;

        public LoopSession(TcpSocket client, ConnectionLoop loop)
        {
            this.client = client;
            this.loop = loop;
        }

        public TcpSocket Client
        {
            get { return client; }
        }

        public ConnectionLoop Loop
        {
            get { return loop; }
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}