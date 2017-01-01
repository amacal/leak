using Leak.Common;

namespace Leak.Events
{
    public class BlockWritten
    {
        public FileHash Hash;

        public BlockIndex Block;
    }
}