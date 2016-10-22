namespace Leak.Suckets
{
    public class TcpSocketReceiveData
    {
        private readonly int received;
        private readonly TcpSocket socket;
        private readonly TcpSocketBuffer buffer;

        public TcpSocketReceiveData(int received, TcpSocket socket, TcpSocketBuffer buffer)
        {
            this.received = received;
            this.socket = socket;
            this.buffer = buffer;
        }

        public TcpSocket Socket
        {
            get { return socket; }
        }

        public TcpSocketBuffer Buffer
        {
            get { return buffer; }
        }

        public int Received
        {
            get { return received; }
        }
    }
}