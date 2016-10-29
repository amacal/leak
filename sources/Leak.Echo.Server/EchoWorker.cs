using Leak.Sockets;

namespace Leak.Echo.Server
{
    public class EchoWorker
    {
        private readonly EchoWorkerFactory factory;
        private readonly byte[] data;

        public EchoWorker(EchoWorkerFactory factory)
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
            if (received.Count > 0 && received.Status == TcpSocketStatus.OK)
            {
                int count = received.Count;
                TcpSocketBuffer buffer = new TcpSocketBuffer(data, 0, count);

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
            if (sent.Count > 0 && sent.Status == TcpSocketStatus.OK)
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