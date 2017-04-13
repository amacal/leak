using System;
using Leak.Sockets;

namespace Leak.Peer.Receiver.Tests
{
    public class LoopSession : IDisposable
    {
        private readonly TcpSocket client;
        private readonly ReceiverService loop;

        public LoopSession(TcpSocket client, ReceiverService loop)
        {
            this.client = client;
            this.loop = loop;
        }

        public TcpSocket Client
        {
            get { return client; }
        }

        public ReceiverService Loop
        {
            get { return loop; }
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}