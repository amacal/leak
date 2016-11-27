using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Events
{
    public class DataReceived
    {
        public FileHash Hash;

        public PeerHash Peer;

        public int Piece;

        public int Block;

        public DataBlock Payload;
    }
}