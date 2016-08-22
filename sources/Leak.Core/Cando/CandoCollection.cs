using Leak.Core.Common;
using System.Collections.Generic;

namespace Leak.Core.Cando
{
    public class CandoCollection
    {
        private readonly Dictionary<PeerSession, CandoEntry> byPeer;

        public CandoCollection()
        {
            byPeer = new Dictionary<PeerSession, CandoEntry>();
        }

        public CandoEntry GetOrCreate(PeerSession session)
        {
            CandoEntry entry;

            if (byPeer.TryGetValue(session, out entry) == false)
            {
                entry = new CandoEntry(session);
                byPeer.Add(session, entry);
            }

            return entry;
        }

        public void Remove(PeerSession session)
        {
            byPeer.Remove(session);
        }
    }
}