using Leak.Common;
using Leak.Core.Messages;

namespace Leak.Core.Events
{
    public class BlockReceived
    {
        public FileHash Hash;

        public PeerHash Peer;

        public int Piece;

        public int Block;

        public DataBlock Payload;
    }
}