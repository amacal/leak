using Leak.Core.Common;
using System.Collections.Generic;

namespace Leak.Core.Battlefield
{
    public class BattlefieldCollection
    {
        private readonly Dictionary<PeerSession, BattlefieldEntry> byPeer;

        public BattlefieldCollection()
        {
            byPeer = new Dictionary<PeerSession, BattlefieldEntry>();
        }

        public BattlefieldEntry GetOrCreate(PeerSession session)
        {
            BattlefieldEntry entry;

            if (byPeer.TryGetValue(session, out entry) == false)
            {
                entry = new BattlefieldEntry(session);
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