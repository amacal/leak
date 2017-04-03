using Leak.Common;

namespace Leak.Networking.Tests
{
    public class NetworkBlock : NetworkPoolMemoryBlock, DataBlock
    {
        private readonly byte[] data;
        private readonly int offset;
        private readonly int count;

        public NetworkBlock(byte[] data, int offset, int count)
        {
            this.data = data;
            this.offset = offset;
            this.count = count;
        }

        public byte[] Data
        {
            get { return data; }
        }

        public int Length
        {
            get { return data.Length; }
        }

        public void Release()
        {
        }

        public void With(DataBlockCallback callback)
        {
            callback.Invoke(data, offset, count);
        }

        public DataBlock Scope(int shift)
        {
            return new NetworkBlock(data, offset + shift, count - shift);
        }
    }
}