using Leak.Core.Common;

namespace Leak.Core.Events
{
    public class PeerBitfieldMessageReceived
    {
        public PeerHash Peer;

        public Bitfield Bitfield;
    }
}