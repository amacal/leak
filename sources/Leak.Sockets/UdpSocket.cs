using System;
using System.Net;
using System.Threading.Tasks;

namespace Leak.Sockets
{
    public interface UdpSocket : IDisposable
    {
        bool Bind();

        bool Bind(int port);

        void Send(IPEndPoint endpoint, SocketBuffer buffer, UdpSocketSendCallback callback);

        Task<UdpSocketSend> Send(IPEndPoint endpoint, SocketBuffer buffer);

        void Receive(SocketBuffer buffer, UdpSocketReceiveCallback callback);

        Task<UdpSocketReceive> Receive(SocketBuffer buffer);
    }
}