using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Memory
{
    public class MemoryBlock : DataBlock
    {
        private readonly byte[] data;
        private readonly int start;
        private readonly int count;

        private readonly MemoryCollection collection;

        public MemoryBlock(byte[] data, int start, int count, MemoryCollection collection)
        {
            this.data = data;
            this.start = start;
            this.count = count;
            this.collection = collection;
        }

        public int Length
        {
            get { return count; }
        }

        public byte[] Data
        {
            get { return data; }
        }

        public byte this[int index]
        {
            get { return data[index + start]; }
        }

        public void With(DataBlockCallback callback)
        {
            callback.Invoke(data, start, count);
        }

        public DataBlock Scope(int shift)
        {
            return new MemoryBlock(data, start + shift, count - shift, collection);
        }

        public void Release()
        {
            collection?.Release(data);
        }
    }
}