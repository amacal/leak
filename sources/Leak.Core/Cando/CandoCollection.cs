using Leak.Core.Common;
using System.Collections.Generic;

namespace Leak.Core.Cando
{
    public class CandoCollection
    {
        private readonly Dictionary<PeerHash, CandoEntry> byPeer;

        public CandoCollection()
        {
            byPeer = new Dictionary<PeerHash, CandoEntry>();
        }

        public CandoEntry GetOrCreate(PeerHash peer)
        {
            CandoEntry entry;

            if (byPeer.TryGetValue(peer, out entry) == false)
            {
                entry = new CandoEntry(peer);
                byPeer.Add(peer, entry);
            }

            return entry;
        }

        public void Remove(PeerHash peer)
        {
            byPeer.Remove(peer);
        }
    }
}