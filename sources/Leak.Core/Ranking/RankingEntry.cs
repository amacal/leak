using Leak.Core.Common;

namespace Leak.Core.Ranking
{
    public class RankingEntry
    {
        private readonly PeerHash peer;

        public RankingEntry(PeerHash peer)
        {
            this.peer = peer;
        }

        public PeerHash Peer
        {
            get { return peer; }
        }

        public int Value { get; set; }
    }
}