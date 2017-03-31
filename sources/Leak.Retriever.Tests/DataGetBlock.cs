using Leak.Common;

namespace Leak.Data.Get.Tests
{
    public class DataGetBlock : DataBlock
    {
        private readonly byte[] data;
        private readonly int offset;

        public DataGetBlock()
        {
            this.data = Bytes.Random(20000);
        }

        private DataGetBlock(byte[] data, int offset)
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
            return new DataGetBlock(data, shift + offset);
        }

        public void Release()
        {
        }
    }
}