using Leak.Memory;
using Leak.Networking;
using Leak.Networking.Core;

namespace Leak.Client.Adapters
{
    internal class MemoryToNetworkAdapter : NetworkPoolMemory
    {
        private readonly MemoryService service;

        public MemoryToNetworkAdapter(MemoryService service)
        {
            this.service = service;
        }

        public NetworkPoolMemoryBlock Allocate(int size)
        {
            return new Block(service.Allocate(size));
        }

        public DataBlockFactory AsFactory()
        {
            return service;
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