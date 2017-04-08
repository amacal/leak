using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Networking.Tests
{
    public class NetworkMemory : NetworkPoolMemory, DataBlockFactory
    {
        public NetworkPoolMemoryBlock Allocate(int size)
        {
            return new NetworkBlock(new byte[size], 0, size);
        }

        public DataBlockFactory AsFactory()
        {
            return this;
        }

        public DataBlock Transcient(byte[] data, int offset, int count)
        {
            return new NetworkBlock(data, offset, count);
        }

        public DataBlock Pooled(int size, DataBlockCallback callback)
        {
            byte[] data = new byte[size];
            DataBlock block = new NetworkBlock(data, 0, size);

            callback?.Invoke(data, 0, size);
            return block;
        }
    }
}