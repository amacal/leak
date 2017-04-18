using Leak.Common;

namespace Leak.Peer.Coordinator.Events
{
    public class BitfieldChanged
    {
        public PeerHash Peer;
        public Bitfield Bitfield;
        public PieceInfo Affected;
    }
}