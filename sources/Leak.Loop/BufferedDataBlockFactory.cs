using System.Collections.Concurrent;
using Leak.Common;

namespace Leak.Loop
{
    public class BufferedBlockFactory : DataBlockFactory
    {
        private readonly int size;
        private readonly ConcurrentQueue<byte[]> buffer;

        public BufferedBlockFactory()
        {
            size = 40000;
            buffer = new ConcurrentQueue<byte[]>();
        }

        public DataBlock Create(byte[] data, int offset, int count)
        {
            return new DataBlockInstance(data, offset, count, null);
        }

        public DataBlock New(int count, DataBlockCallback callback)
        {
            byte[] data;

            if (buffer.TryDequeue(out data) == false)
            {
                data = new byte[size];
            }

            callback?.Invoke(data, 0, count);
            return new DataBlockInstance(data, 0, count, buffer);
        }

        private class DataBlockInstance : DataBlock
        {
            private readonly byte[] data;
            private readonly int start;
            private readonly int count;
            private readonly ConcurrentQueue<byte[]> buffer;

            public DataBlockInstance(byte[] data, int start, int count, ConcurrentQueue<byte[]> buffer)
            {
                this.data = data;
                this.start = start;
                this.count = count;
                this.buffer = buffer;
            }

            public int Size
            {
                get { return count; }
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
                return new DataBlockInstance(data, start + shift, count - shift, buffer);
            }

            public void Dispose()
            {
                buffer.Enqueue(data);
            }
        }
    }
}