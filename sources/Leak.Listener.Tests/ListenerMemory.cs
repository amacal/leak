using Leak.Networking;

namespace Leak.Listener.Tests
{
    public class ListenerMemory : NetworkPoolMemory
    {
        public NetworkPoolMemoryBlock Allocate(int size)
        {
            return new Block(size);
        }

        private class Block : NetworkPoolMemoryBlock
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