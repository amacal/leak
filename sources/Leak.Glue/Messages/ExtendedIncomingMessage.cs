using Leak.Common;

namespace Leak.Peer.Coordinator.Messages
{
    public class ExtendedIncomingMessage
    {
        private readonly byte[] data;

        public ExtendedIncomingMessage(byte[] data)
        {
            this.data = data;
        }

        public byte Id
        {
            get { return data[0]; }
        }

        public int Size
        {
            get { return data.Length - 1; }
        }

        public byte[] ToBytes()
        {
            return Bytes.Copy(data, 1);
        }

        public byte[] ToBytes(int offset)
        {
            return Bytes.Copy(data, offset + 1);
        }
    }
}