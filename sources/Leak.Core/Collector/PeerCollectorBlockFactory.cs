using Leak.Core.Messages;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace Leak.Core.Collector
{
    public class PeerCollectorBlockFactory : DataBlockFactory
    {
        private readonly byte[] buffer;
        private readonly ConcurrentQueue<int> map;

        public PeerCollectorBlockFactory()
        {
            buffer = new byte[40000 * 128];
            map = new ConcurrentQueue<int>(Enumerable.Range(0, 128));
        }

        public DataBlock Create(byte[] data, int offset, int count)
        {
            return new DataBlockInstance(data, offset, count, null);
        }

        public DataBlock New(int count, Action<byte[], int, int> callback)
        {
            int index = 0;

            while (map.TryDequeue(out index) == false)
            {
                Thread.Sleep(100);
            }

            callback.Invoke(buffer, index * 40000, count);
            return new DataBlockInstance(buffer, index * 40000, count, () => map.Enqueue(index));
        }

        private class DataBlockInstance : DataBlock
        {
            private readonly byte[] data;
            private readonly int start;
            private readonly int count;
            private readonly Action onCompleted;

            public DataBlockInstance(byte[] data, int start, int count, Action onCompleted)
            {
                this.data = data;
                this.start = start;
                this.count = count;
                this.onCompleted = onCompleted;
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
                return new DataBlockInstance(data, start + offset, count - offset, onCompleted);
            }

            public void Dispose()
            {
                onCompleted?.Invoke();
            }
        }
    }
}