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

        public FileHash Hash { get; set; }
    }
}