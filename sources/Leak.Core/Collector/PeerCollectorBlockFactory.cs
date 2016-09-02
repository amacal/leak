using Leak.Core.Messages;
using System;

namespace Leak.Core.Collector
{
    public class PeerCollectorBlockFactory : DataBlockFactory
    {
        public DataBlock Create(byte[] data, int offset, int count)
        {
            return new DataBlockInstance(data, offset, count);
        }

        private class DataBlockInstance : DataBlock
        {
            private readonly byte[] data;
            private readonly int start;
            private readonly int count;

            public DataBlockInstance(byte[] data, int start, int count)
            {
                this.data = data;
                this.start = start;
                this.count = count;
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
                return new DataBlockInstance(data, start + offset, count - offset);
            }
        }
    }
}