using Leak.Common;

namespace Leak.Memory
{
    public class MemoryBlock : DataBlock
    {
        private readonly byte[] data;
        private readonly int start;
        private readonly int count;

        private readonly MemoryContext context;

        public MemoryBlock(byte[] data, int start, int count, MemoryContext context)
        {
            this.data = data;
            this.start = start;
            this.count = count;
            this.context = context;
        }

        public int Size
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

        public void Write(DataBlockCallback callback)
        {
            callback.Invoke(data, start, count);
        }

        public DataBlock Scope(int shift)
        {
            return new MemoryBlock(data, start + shift, count - shift, context);
        }

        public void Release()
        {
            context?.Buffer.Enqueue(data);
        }
    }
}