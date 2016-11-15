using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Events
{
    public class PeerPieceMessageReceived
    {
        public PeerHash Peer;

        public DataBlock Data;
    }
}