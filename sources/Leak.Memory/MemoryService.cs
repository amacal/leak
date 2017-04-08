using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Memory
{
    public class MemoryService : DataBlockFactory
    {
        private readonly MemoryContext context;

        public MemoryService(MemoryParameters parameters, MemoryDependencies dependencies, MemoryConfiguration configuration, MemoryHooks hooks)
        {
            context = new MemoryContext(parameters, dependencies, configuration, hooks);
        }

        public MemoryBlock Allocate(int size)
        {
            return context.Collection.Allocate(size);
        }

        public DataBlock Transcient(byte[] data, int offset, int count)
        {
            return new MemoryBlock(data, offset, count, null);
        }

        public DataBlock Pooled(int size, DataBlockCallback callback)
        {
            MemoryBlock found = context.Collection.Allocate(size);

            callback?.Invoke(found.Data, 0, size);

            return found;
        }

        public void Release(byte[] data)
        {
            context.Collection.Release(data);
        }
    }
}