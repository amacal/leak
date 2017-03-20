using System;
using System.Net;
using System.Threading.Tasks;

namespace Leak.Sockets
{
    public interface TcpSocket : IDisposable
    {
        bool Bind();

        bool Bind(int port);

        bool Bind(IPAddress address);

        TcpSocketInfo Info();

        void Listen(int backlog);

        void Accept(TcpSocketAcceptCallback callback);

        Task<TcpSocketAccept> Accept();

        void Connect(IPEndPoint endpoint, TcpSocketConnectCallback callback);

        Task<TcpSocketConnect> Connect(IPEndPoint endpoint);

        void Disconnect(TcpSocketDisconnectCallback callback);

        Task<TcpSocketDisconnect> Disconnect();

        void Send(SocketBuffer buffer, TcpSocketSendCallback callback);

        Task<TcpSocketSend> Send(SocketBuffer buffer);

        void Receive(SocketBuffer buffer, TcpSocketReceiveCallback callback);

        Task<TcpSocketReceive> Receive(SocketBuffer buffer);
    }
}