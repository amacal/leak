using System.Net;

namespace Leak.Sockets
{
    internal class UdpSocketSendResult : SocketResult
    {
        public UdpSocket Socket { get; set; }

        public SocketBuffer Buffer { get; set; }

        public UdpSocketSendCallback OnSent { get; set; }

        public IPEndPoint Endpoint { get; set; }

        public UdpSocketSend CreateData()
        {
            return new UdpSocketSend(Status, Affected, Socket, Buffer, Endpoint);
        }

        protected override void OnCompleted(int affected)
        {
            OnSent?.Invoke(new UdpSocketSend(Status, Affected, Socket, Buffer, Endpoint));
        }

        protected override void OnFailed(SocketStatus status)
        {
            OnSent?.Invoke(new UdpSocketSend(Status, Affected, Socket, Buffer, Endpoint));
        }
    }
}