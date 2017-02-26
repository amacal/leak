using System;

namespace Leak.Sockets
{
    internal class TcpSocketDisconnectResult : SocketResult
    {
        public TcpSocket Socket { get; set; }

        public TcpSocketDisconnectCallback OnDisconnected { get; set; }

        public TcpSocketDisconnect Unpack(IAsyncResult result)
        {
            return new TcpSocketDisconnect(Status, Socket);
        }

        public TcpSocketDisconnect CreateData()
        {
            return new TcpSocketDisconnect(Status, Socket);
        }

        protected override void OnCompleted(int affected)
        {
            OnDisconnected?.Invoke(new TcpSocketDisconnect(Status, Socket));
        }

        protected override void OnFailed(SocketStatus status)
        {
            OnDisconnected?.Invoke(new TcpSocketDisconnect(Status, Socket));
        }
    }
}