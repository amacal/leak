using System;
using System.Net;

namespace Leak.Sockets.Tests
{
    public class EchoServer : IDisposable
    {
        private readonly TcpSocket socket;

        public EchoServer(SocketFactory factory)
        {
            socket = factory.Tcp();
            socket.Bind(IPAddress.Loopback);
        }

        public IPEndPoint Endpoint
        {
            get { return socket.Info().Endpoint; }
        }

        public void Start()
        {
            socket.Listen(10);
            socket.Accept(OnAccepted);
        }

        private async void OnAccepted(TcpSocketAccept data)
        {
            if (data.Status == SocketStatus.OK)
            {
                socket.Accept(OnAccepted);
            }

            if (data.Status == SocketStatus.OK)
            {
                TcpSocket other = data.Connection;
                byte[] bytes = new byte[1024];

                while (true)
                {
                    TcpSocketReceive received = await other.Receive(bytes);
                    SocketBuffer buffer = new SocketBuffer(bytes, 0, received.Count);

                    if (received.Count == 0)
                        break;

                    if (received.Status != SocketStatus.OK)
                        break;

                    TcpSocketSend sent = await other.Send(buffer);

                    if (sent.Status != SocketStatus.OK)
                        break;
                }

                other.Dispose();
            }
        }

        public void Dispose()
        {
            socket.Dispose();
        }
    }
}