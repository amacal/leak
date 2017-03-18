using System;

namespace Leak.Memory
{
    public class MemoryConfiguration
    {
        public MemoryConfiguration()
        {
            Size = 32 * 1024;
            Delay = TimeSpan.FromMinutes(1);
        }

        public int Size;
        public TimeSpan Delay;
    }
}