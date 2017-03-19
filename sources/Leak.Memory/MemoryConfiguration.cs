using System;

namespace Leak.Memory
{
    public class MemoryConfiguration
    {
        public MemoryConfiguration()
        {
            MaxBlockSize = 32 * 1024;
            Thresholds = new int[0];
        }

        public int MaxBlockSize;
        public int[] Thresholds;
    }
}