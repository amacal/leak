using Leak.Common;

namespace Leak.Events
{
    public class BlockRead
    {
        public FileHash Hash;

        public BlockIndex Block;

        public DataBlock Payload;
    }
}
