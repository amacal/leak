using Leak.Sockets;
using System;

namespace Leak.Echo.Server
{
    public class EchoServer : IDisposable
    {
        private readonly TcpSocket socket;
        private readonly EchoWorkerFactory workers;

        public EchoServer(TcpSocketFactory factory, int port)
        {
            workers = new EchoWorkerFactory();
            socket = factory.Create();

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

            if (data.Status == TcpSocketStatus.OK)
            {
                workers.Next().Handle(data.Connection);
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