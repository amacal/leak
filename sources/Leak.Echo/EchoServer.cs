using Leak.Sockets;
using System;

namespace Leak.Echo
{
    public class EchoServer : IDisposable
    {
        private readonly TcpSocket socket;
        private readonly EchoServerWorkerFactory serverWorkers;

        public EchoServer(SocketFactory factory, int port)
        {
            serverWorkers = new EchoServerWorkerFactory();
            socket = factory.Tcp();

            socket.Bind(port);
        }

        public void Start()
        {
            socket.Listen(10);
            socket.Accept(OnAccepted);
        }

        private void OnAccepted(TcpSocketAccept data)
        {
            socket.Accept(OnAccepted);

            if (data.Status == SocketStatus.OK)
            {
                serverWorkers.Next().Handle(data.Connection);
            }
            else
            {
                data.Connection.Dispose();
            }
        }

        public void Dispose()
        {
            socket.Dispose();
        }
    }
}