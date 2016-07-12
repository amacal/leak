namespace Leak.Core.Network
{
    public class NetworkOutgoingMessage
    {
        private readonly byte[] data;
        private readonly int length;

        public NetworkOutgoingMessage(params byte[] data)
        {
            this.data = data;
            this.length = data.Length;
        }

        public NetworkOutgoingMessage(NetworkBuffer buffer)
        {
            this.length = buffer.Length;
            this.data = buffer.ToBytes();
        }

        public byte this[int index]
        {
            get { return data[index]; }
        }

        public int Length
        {
            get { return length; }
        }

        public byte[] ToBytes()
        {
            return data;
        }
    }
}