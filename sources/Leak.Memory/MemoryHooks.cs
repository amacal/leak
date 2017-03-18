using System;
using Leak.Memory.Events;

namespace Leak.Memory
{
    public class MemoryHooks
    {
        public Action<MemorySnapshot> OnMemorySnapshot;
    }
}