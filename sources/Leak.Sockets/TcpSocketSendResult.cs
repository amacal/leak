namespace Leak.Sockets
{
    internal class TcpSocketSendResult : SocketResult
    {
        public TcpSocket Socket { get; set; }

        public SocketBuffer Buffer { get; set; }

        public TcpSocketSendCallback OnSent { get; set; }

        public TcpSocketSend CreateData()
        {
            return new TcpSocketSend(Status, Affected, Socket, Buffer);
        }

        protected override void OnCompleted(int affected)
        {
            OnSent?.Invoke(new TcpSocketSend(Status, Affected, Socket, Buffer));
        }

        protected override void OnFailed(SocketStatus status)
        {
            OnSent?.Invoke(new TcpSocketSend(Status, Affected, Socket, Buffer));
        }
    }
}