namespace Leak.Sockets
{
    internal class UdpSocketReceiveResult : SocketResult
    {
        public UdpSocket Socket { get; set; }

        public SocketBuffer Buffer { get; set; }

        public UdpSocketReceiveCallback OnReceived { get; set; }

        public byte[] Address { get; set; }

        public UdpSocketReceive CreateData()
        {
            return new UdpSocketReceive(Status, Affected, Socket, Buffer, Address);
        }

        protected override void OnCompleted(int affected)
        {
            OnReceived?.Invoke(new UdpSocketReceive(Status, Affected, Socket, Buffer, Address));
        }

        protected override void OnFailed(SocketStatus status)
        {
            OnReceived?.Invoke(new UdpSocketReceive(Status, Affected, Socket, Buffer, Address));
        }
    }
}