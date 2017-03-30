using Leak.Common;

namespace Leak.Data.Share.Tests
{
    public class DataShareBlock : DataBlock
    {
        private readonly int size;

        public DataShareBlock(int size)
        {
            this.size = size;
        }

        public int Size
        {
            get { return size; }
        }

        public byte this[int index]
        {
            get { return 0; }
        }

        public void Write(DataBlockCallback callback)
        {
            callback.Invoke(new byte[size], 0, size);
        }

        public DataBlock Scope(int shift)
        {
            throw new System.NotImplementedException();
        }

        public void Release()
        {
        }
    }
}