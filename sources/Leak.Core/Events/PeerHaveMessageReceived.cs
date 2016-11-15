using Leak.Core.Common;

namespace Leak.Core.Events
{
    public class PeerHaveMessageReceived
    {
        public PeerHash Peer;

        public int Piece;
    }
}