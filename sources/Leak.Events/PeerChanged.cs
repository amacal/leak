using Leak.Common;

namespace Leak.Events
{
    public class PeerChanged
    {
        public PeerHash Peer;
        public Bitfield Bitfield;
        public PeerState State;
    }
}