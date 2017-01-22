using Leak.Common;

namespace Leak.Datashare.Tests
{
    public class DatashareBlock : DataBlock
    {
        private readonly byte[] data;
        private readonly int offset;

        public DatashareBlock(byte[] data)
        {
            this.data = data;
        }

        private DatashareBlock(byte[] data, int offset)
        {
            this.data = data;
            this.offset = offset;
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
            return new DatashareBlock(data, shift + offset);
        }

        public void Dispose()
        {
        }
    }
}