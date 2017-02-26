namespace Leak.Sockets
{
    internal class TcpSocketReceiveResult : SocketResult
    {
        public TcpSocket Socket { get; set; }

        public SocketBuffer Buffer { get; set; }

        public TcpSocketReceiveCallback OnReceived { get; set; }

        public TcpSocketReceive CreateData()
        {
            return new TcpSocketReceive(Status, Affected, Socket, Buffer);
        }

        protected override void OnCompleted(int affected)
        {
            OnReceived?.Invoke(new TcpSocketReceive(Status, Affected, Socket, Buffer));
        }

        protected override void OnFailed(SocketStatus status)
        {
            OnReceived?.Invoke(new TcpSocketReceive(Status, Affected, Socket, Buffer));
        }
    }
}