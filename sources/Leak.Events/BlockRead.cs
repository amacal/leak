using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Events
{
    public class BlockRead
    {
        public FileHash Hash;

        public BlockIndex Block;

        public DataBlock Payload;
    }
}