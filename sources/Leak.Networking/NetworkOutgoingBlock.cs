using Leak.Networking.Core;

namespace Leak.Networking
{
    public class NetworkOutgoingBlock : DataBlock
    {
        private readonly byte[] data;
        private readonly int offset;
        private readonly int length;

        public NetworkOutgoingBlock(byte[] data, int offset, int length)
        {
            this.data = data;
            this.offset = offset;
            this.length = length;
        }

        public int Length
        {
            get { return length; }
        }

        public void With(DataBlockCallback callback)
        {
            callback.Invoke(data, offset, length);
        }

        public DataBlock Scope(int shift)
        {
            return new NetworkOutgoingBlock(data, offset + shift, length - shift);
        }

        public void Release()
        {
        }
    }
}