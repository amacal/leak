using Leak.Common;

namespace Leak.Data.Share.Tests
{
    public class DataShareBlock : DataBlock
    {
        private readonly byte[] data;
        private readonly int offset;

        public DataShareBlock(byte[] data)
        {
            this.data = data;
        }

        private DataShareBlock(byte[] data, int offset)
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
            return new DataShareBlock(data, shift + offset);
        }

        public void Release()
        {
        }
    }
}