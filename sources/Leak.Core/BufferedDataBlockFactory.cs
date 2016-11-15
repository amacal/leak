using Leak.Core.Messages;
using System;
using System.Collections.Concurrent;

namespace Leak.Core
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

        public DataBlock New(int count, Action<byte[], int, int> callback)
        {
            byte[] data;

            if (buffer.TryDequeue(out data) == false)
            {
                data = new byte[size];
            }

            callback.Invoke(data, 0, count);
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

            public void Write(Action<byte[], int, int> stream)
            {
                stream.Invoke(data, start, count);
            }

            public DataBlock Scope(int offset)
            {
                return new DataBlockInstance(data, start + offset, count - offset, buffer);
            }

            public void Dispose()
            {
                buffer.Enqueue(data);
            }
        }
    }
}