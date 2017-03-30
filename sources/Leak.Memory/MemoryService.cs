using System;
using Leak.Common;

namespace Leak.Memory
{
    public class MemoryService : DataBlockFactory
    {
        private readonly MemoryContext context;

        public MemoryService(MemoryParameters parameters, MemoryDependencies dependencies, MemoryConfiguration configuration, MemoryHooks hooks)
        {
            context = new MemoryContext(parameters, dependencies, configuration, hooks);
        }

        public DataBlock Create(byte[] data, int offset, int count)
        {
            return new MemoryBlock(data, offset, count, null);
        }

        public MemoryBlock Allocate(int size)
        {
            return context.Collection.Allocate(size);
        }

        public DataBlock New(int count, DataBlockCallback callback)
        {
            MemoryBlock found = context.Collection.Allocate(count);

            callback?.Invoke(found.Data, 0, count);

            return found;
        }

        public void Release(byte[] data)
        {
            context.Collection.Release(data);
        }
    }
}