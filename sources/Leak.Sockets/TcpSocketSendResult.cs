namespace Leak.Sockets
{
    internal class TcpSocketSendResult : TcpSocketResult
    {
        public TcpSocket Socket { get; set; }

        public TcpSocketBuffer Buffer { get; set; }

        public TcpSocketSendCallback OnSent { get; set; }

        public TcpSocketSend CreateData()
        {
            return new TcpSocketSend(Status, Affected, Socket, Buffer);
        }

        protected override void OnCompleted(int affected)
        {
            OnSent?.Invoke(new TcpSocketSend(Status, Affected, Socket, Buffer));
        }

        protected override void OnFailed(TcpSocketStatus status)
        {
            OnSent?.Invoke(new TcpSocketSend(Status, Affected, Socket, Buffer));
        }
    }
}