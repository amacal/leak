using System;
using System.Net;
using System.Threading.Tasks;

namespace Leak.Sockets
{
    public interface UdpSocket : IDisposable
    {
        void Bind();

        void Bind(int port);

        void SetTimeout(int seconds);

        void Send(IPEndPoint endpoint, SocketBuffer buffer, UdpSocketSendCallback callback);

        Task<UdpSocketSend> Send(IPEndPoint endpoint, SocketBuffer buffer);

        void Receive(SocketBuffer buffer, UdpSocketReceiveCallback callback);

        Task<UdpSocketReceive> Receive(SocketBuffer buffer);
    }
}