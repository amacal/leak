namespace Leak.Sockets
{
    public class SocketBuffer
    {
        private readonly byte[] data;
        private readonly int offset;
        private readonly int count;

        public SocketBuffer(int size)
        {
            this.offset = 0;
            this.count = size;
            this.data = new byte[size];
        }

        public SocketBuffer(byte[] data)
        {
            this.data = data;
            this.offset = 0;
            this.count = data.Length;
        }

        public SocketBuffer(byte[] data, int offset)
        {
            this.data = data;
            this.offset = offset;
            this.count = data.Length - offset;
        }

        public SocketBuffer(byte[] data, int offset, int count)
        {
            this.data = data;
            this.offset = offset;
            this.count = count;
        }

        public byte this[int index]
        {
            get { return data[index + offset]; }
            set { data[index + offset] = value; }
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

        public static implicit operator SocketBuffer(byte[] data)
        {
            return new SocketBuffer(data);
        }
    }
}