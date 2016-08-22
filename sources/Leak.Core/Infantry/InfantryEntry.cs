using Leak.Core.Common;

namespace Leak.Core.Infantry
{
    public class InfantryEntry
    {
        public InfantryEntry(PeerHash peer)
        {
            Peer = peer;
        }

        public PeerHash Peer { get; set; }

        public PeerAddress Address { get; set; }

        public PeerSession Session { get; set; }
    }
}