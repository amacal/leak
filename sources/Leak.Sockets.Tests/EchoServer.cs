using System;
using System.Net;

namespace Leak.Sockets.Tests
{
    public class EchoServer : IDisposable
    {
        private readonly TcpSocket socket;

        public EchoServer(TcpSocketFactory factory)
        {
            socket = factory.Create();
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
            if (data.Status == TcpSocketStatus.OK)
            {
                socket.Accept(OnAccepted);
            }

            if (data.Status == TcpSocketStatus.OK)
            {
                TcpSocket other = data.Connection;
                byte[] bytes = new byte[1024];

                while (true)
                {
                    TcpSocketReceive received = await other.Receive(bytes);
                    TcpSocketBuffer buffer = new TcpSocketBuffer(bytes, 0, received.Count);

                    if (received.Count == 0)
                        break;

                    if (received.Status != TcpSocketStatus.OK)
                        break;

                    TcpSocketSend sent = await other.Send(buffer);

                    if (sent.Status != TcpSocketStatus.OK)
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