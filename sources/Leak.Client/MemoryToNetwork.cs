using Leak.Memory;
using Leak.Networking;

namespace Leak.Client
{
    public class MemoryToNetwork : NetworkPoolMemory
    {
        private readonly MemoryService service;

        public MemoryToNetwork(MemoryService service)
        {
            this.service = service;
        }

        public NetworkPoolMemoryBlock Allocate(int size)
        {
            return new Block(service.Allocate(size));
        }

        private class Block : NetworkPoolMemoryBlock
        {
            private readonly MemoryBlock inner;

            public Block(MemoryBlock inner)
            {
                this.inner = inner;
            }

            public byte[] Data
            {
                get { return inner.Data; }
            }

            public int Length
            {
                get { return inner.Data.Length; }
            }

            public void Release()
            {
                inner.Release();
            }
        }
    }
}