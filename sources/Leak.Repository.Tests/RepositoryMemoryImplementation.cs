namespace Leak.Data.Store.Tests
{
    public class RepositoryMemoryImplementation : RepositoryMemory
    {
        public RepositoryMemoryBlock Allocate(int size)
        {
            return new Block(size);
        }

        private class Block : RepositoryMemoryBlock
        {
            private readonly byte[] data;

            public Block(int size)
            {
                data = new byte[size];
            }

            public byte[] Data
            {
                get { return data; }
            }

            public int Length
            {
                get { return data.Length; }
            }

            public void Release()
            {
            }
        }
    }
}