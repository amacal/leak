using Leak.Common;

namespace Leak.Events
{
    public class PeerBitfieldChanged
    {
        public PeerHash Peer;
        public Bitfield Bitfield;
        public PieceInfo Affected;
    }
}