namespace Leak.Sockets
{
    public class TcpSocketBuffer
    {
        private readonly byte[] data;
        private readonly int offset;
        private readonly int count;

        public TcpSocketBuffer(byte[] data)
        {
            this.data = data;
            this.offset = 0;
            this.count = data.Length;
        }

        public TcpSocketBuffer(byte[] data, int offset)
        {
            this.data = data;
            this.offset = offset;
            this.count = data.Length - offset;
        }

        public TcpSocketBuffer(byte[] data, int offset, int count)
        {
            this.data = data;
            this.offset = offset;
            this.count = count;
        }

        public byte[] Data
        {
            get { return data; }
        }

        public int Offset
        {
            get { return offset; }
        }

        public int Count
        {
            get { return count; }
        }

        public static implicit operator TcpSocketBuffer(byte[] data)
        {
            return new TcpSocketBuffer(data);
        }
    }
}