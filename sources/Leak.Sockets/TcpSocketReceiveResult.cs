namespace Leak.Sockets
{
    internal class TcpSocketReceiveResult : TcpSocketResult
    {
        public TcpSocket Socket { get; set; }

        public TcpSocketBuffer Buffer { get; set; }

        public TcpSocketReceiveCallback OnReceived { get; set; }

        public TcpSocketReceive CreateData()
        {
            return new TcpSocketReceive(Status, Affected, Socket, Buffer);
        }

        protected override void OnCompleted(int affected)
        {
            OnReceived?.Invoke(new TcpSocketReceive(Status, Affected, Socket, Buffer));
        }

        protected override void OnFailed(TcpSocketStatus status)
        {
            OnReceived?.Invoke(new TcpSocketReceive(Status, Affected, Socket, Buffer));
        }
    }
}