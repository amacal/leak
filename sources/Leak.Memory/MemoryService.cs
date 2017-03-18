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
            byte[] data;

            if (size > context.Configuration.Size)
            {
                throw new InvalidOperationException();
            }

            if (context.Buffer.TryDequeue(out data) == false)
            {
                data = new byte[context.Configuration.Size];

                lock (this)
                {
                    context.Count = context.Count + 1;
                    context.Allocation = context.Allocation.Increase(data.Length);
                }

                context.CallSnapshot(context.Count, context.Allocation);
            }

            return new MemoryBlock(data, 0, size, context);
        }

        public DataBlock New(int count, DataBlockCallback callback)
        {
            byte[] data;

            if (context.Buffer.TryDequeue(out data) == false)
            {
                data = new byte[context.Configuration.Size];

                lock (this)
                {
                    context.Count = context.Count + 1;
                    context.Allocation = context.Allocation.Increase(data.Length);
                }

                context.CallSnapshot(context.Count, context.Allocation);
            }

            callback?.Invoke(data, 0, count);
            return new MemoryBlock(data, 0, count, context);
        }
    }
}