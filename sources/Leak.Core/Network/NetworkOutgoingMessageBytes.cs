using Leak.Common;

namespace Leak.Core.Network
{
    public class NetworkOutgoingMessageBytes : NetworkOutgoingMessage
    {
        private readonly int length;
        private readonly byte[] data;

        public NetworkOutgoingMessageBytes(params byte[] data)
        {
            this.data = data;
            this.length = data.Length;
        }

        public NetworkOutgoingMessageBytes(params byte[][] data)
        {
            this.data = Bytes.Concatenate(data);
            this.length = data.Length;
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