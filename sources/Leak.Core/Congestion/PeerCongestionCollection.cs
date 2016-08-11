using Leak.Core.Common;
using System.Collections.Generic;

namespace Leak.Core.Congestion
{
    public class PeerCongestionCollection
    {
        private readonly Dictionary<PeerHash, PeerCongestionEntry> byPeer;

        public PeerCongestionCollection()
        {
            this.byPeer = new Dictionary<PeerHash, PeerCongestionEntry>();
        }

        public PeerCongestionEntry GetOrCreate(PeerHash peer)
        {
            PeerCongestionEntry entry;

            if (byPeer.TryGetValue(peer, out entry) == false)
            {
                entry = new PeerCongestionEntry();
                byPeer.Add(peer, entry);
            }

            return entry;
        }
    }
}