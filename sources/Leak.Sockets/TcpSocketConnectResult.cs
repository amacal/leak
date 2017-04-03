using System;
using System.Net;

namespace Leak.Sockets
{
    internal class TcpSocketConnectResult : SocketResult
    {
        public TcpSocket Socket { get; set; }

        public IPEndPoint Endpoint { get; set; }

        public TcpSocketConnectCallback OnConnected { get; set; }

        public TcpSocketConnect Unpack(IAsyncResult result)
        {
            return new TcpSocketConnect(Status, Socket, Endpoint);
        }

        protected override void OnCompleted()
        {
            OnConnected?.Invoke(new TcpSocketConnect(Status, Socket, Endpoint));
        }

        protected override void OnFailed()
        {
            OnConnected?.Invoke(new TcpSocketConnect(Status, Socket, Endpoint));
        }
    }
}