using Leak.Common;
using Leak.Memory.Events;

namespace Leak.Memory
{
    public static class MemoryExtensions
    {
        public static void CallSnapshot(this MemoryContext context, int count, Size allocation)
        {
            context.Hooks.OnMemorySnapshot?.Invoke(new MemorySnapshot
            {
                Count = count,
                Allocation = allocation
            });
        }
    }
}