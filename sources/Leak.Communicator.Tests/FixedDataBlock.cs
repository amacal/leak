using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Communicator.Tests
{
    public class FixedDataBlock : DataBlock
    {
        private readonly byte[] data;
        private readonly int offset;

        public FixedDataBlock(byte[] data)
        {
            this.data = data;
        }

        private FixedDataBlock(byte[] data, int offset)
        {
            this.data = data;
            this.offset = offset;
        }

        public int Length
        {
            get { return data.Length - offset; }
        }

        public byte this[int index]
        {
            get { return data[index + offset]; }
        }

        public void With(DataBlockCallback callback)
        {
            callback.Invoke(data, offset, data.Length - offset);
        }

        public DataBlock Scope(int shift)
        {
            return new FixedDataBlock(data, shift + offset);
        }

        public void Release()
        {
        }
    }
}