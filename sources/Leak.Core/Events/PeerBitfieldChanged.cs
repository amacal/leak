using Leak.Core.Common;

namespace Leak.Core.Events
{
    public class PeerBitfieldChanged
    {
        public PeerHash Peer;

        public Bitfield Bitfield;
    }
}