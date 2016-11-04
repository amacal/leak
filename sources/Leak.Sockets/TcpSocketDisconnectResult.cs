using System;

namespace Leak.Sockets
{
    internal class TcpSocketDisconnectResult : TcpSocketResult
    {
        public TcpSocket Socket { get; set; }

        public TcpSocketDisconnectCallback OnDisconnected { get; set; }

        public TcpSocketDisconnect Unpack(IAsyncResult result)
        {
            return new TcpSocketDisconnect(Status, Socket);
        }

        protected override void OnCompleted(int affected)
        {
            OnDisconnected?.Invoke(new TcpSocketDisconnect(Status, Socket));
        }

        protected override void OnFailed(TcpSocketStatus status)
        {
            OnDisconnected?.Invoke(new TcpSocketDisconnect(Status, Socket));
        }
    }
}