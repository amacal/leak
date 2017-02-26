using Leak.Sockets;

namespace Leak.Echo
{
    public class EchoServerWorker
    {
        private readonly EchoServerWorkerFactory factory;
        private readonly byte[] data;

        public EchoServerWorker(EchoServerWorkerFactory factory)
        {
            this.factory = factory;
            this.data = new byte[16384];
        }

        public void Handle(TcpSocket socket)
        {
            socket.Receive(data, OnReceived);
        }

        private void OnReceived(TcpSocketReceive received)
        {
            if (received.Count > 0 && received.Status == SocketStatus.OK)
            {
                int count = received.Count;
                SocketBuffer buffer = new SocketBuffer(data, 0, count);

                received.Socket.Send(buffer, OnSent);
            }
            else
            {
                factory.Release(this);
                received.Socket.Dispose();
            }
        }

        private void OnSent(TcpSocketSend sent)
        {
            if (sent.Count > 0 && sent.Status == SocketStatus.OK)
            {
                sent.Socket.Receive(data, OnReceived);
            }
            else
            {
                factory.Release(this);
                sent.Socket.Dispose();
            }
        }
    }
}