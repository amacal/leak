using Leak.Common;
using Leak.Networking;

namespace Leak.Negotiator.Tests
{
    public class NegotiatorMemory : NetworkPoolMemory, DataBlockFactory
    {
        public NetworkPoolMemoryBlock Allocate(int size)
        {
            return new Block(new byte[size], 0, size);
        }

        public DataBlockFactory AsFactory()
        {
            return this;
        }

        public DataBlock Transcient(byte[] data, int offset, int count)
        {
            return new Block(data, offset, count);
        }

        public DataBlock Pooled(int size, DataBlockCallback callback)
        {
            byte[] data = new byte[size];
            DataBlock block = new Block(data, 0, size);

            callback?.Invoke(data, 0, size);
            return block;
        }

        private class Block : NetworkPoolMemoryBlock, DataBlock
        {
            private readonly byte[] data;
            private readonly int offset;
            private readonly int count;

            public Block(byte[] data, int offset, int count)
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
                return new Block(data, offset + shift, count - shift);
            }
        }
    }
}