namespace Leak.Suckets
{
    public class TcpSocketSendData
    {
        private readonly TcpSocket socket;
        private readonly byte[] buffer;

        public TcpSocketSendData(TcpSocket socket, byte[] buffer)
        {
            this.socket = socket;
            this.buffer = buffer;
        }

        public TcpSocket Socket
        {
            get { return socket; }
        }

        public byte[] Buffer
        {
            get { return buffer; }
        }
    }
}