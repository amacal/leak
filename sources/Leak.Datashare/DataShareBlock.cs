using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Data.Share
{
    public class DataShareBlock : DataBlock
    {
        private readonly DataBlock inner;
        private readonly Counter counter;

        public DataShareBlock(DataBlock inner, int count)
        {
            this.inner = inner;
            this.counter = new Counter(count);
        }

        private DataShareBlock(DataBlock inner, Counter counter)
        {
            this.inner = inner;
            this.counter = counter;
        }

        public int Length
        {
            get { return inner.Length; }
        }

        public void With(DataBlockCallback callback)
        {
            inner.With(callback);
        }

        public DataBlock Scope(int shift)
        {
            return new DataShareBlock(inner.Scope(shift), counter);
        }

        public void Release()
        {
            if (counter.Decrease() == 0)
            {
                inner.Release();
            }
        }

        private class Counter
        {
            private int value;

            public Counter(int value)
            {
                this.value = value;
            }

            public int Decrease()
            {
                return --value;
            }
        }
    }
}