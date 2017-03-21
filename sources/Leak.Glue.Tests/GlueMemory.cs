using Leak.Common;
using Leak.Networking;

namespace Leak.Glue.Tests
{
    public class GlueMemory : DataBlockFactory, NetworkPoolMemory
    {
        public DataBlock Create(byte[] data, int offset, int count)
        {
            return new Block(count, null);
        }

        public DataBlock New(int count, DataBlockCallback callback)
        {
            return new Block(count, callback);
        }

        public NetworkPoolMemoryBlock Allocate(int size)
        {
            return new Block(size, null);
        }

        private class Block : DataBlock, NetworkPoolMemoryBlock
        {
            private readonly byte[] data;
            private readonly int offset;

            public Block(int count, DataBlockCallback callback)
            {
                data = new byte[count];
                callback?.Invoke(data, 0, count);
            }

            private Block(Block block, int offset)
            {
                this.data = block.data;
                this.offset = offset + block.offset;
            }

            public int Size
            {
                get { return data.Length - offset; }
            }

            public byte this[int index]
            {
                get { return data[index + offset]; }
            }

            public void Write(DataBlockCallback callback)
            {
                callback.Invoke(data, offset, data.Length - offset);
            }

            public DataBlock Scope(int shift)
            {
                return new Block(this, offset);
            }

            public void Release()
            {
            }

            public byte[] Data
            {
                get { return data; }
            }

            public int Length
            {
                get { return data.Length; }
            }
        }
    }
}