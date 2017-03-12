using Leak.Common;

namespace Leak.Data.Map.Components
{
    public class OmnibusStateEntry
    {
        public OmnibusStateEntry(PeerHash peer)
        {
            Peer = peer;
            State = new PeerState();
        }

        public PeerHash Peer;
        public PeerState State;
        public int Ranking;
    }
}