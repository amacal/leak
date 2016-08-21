using Leak.Core.Common;
using System.Collections.Generic;

namespace Leak.Core.Battlefield
{
    public class BattlefieldCollection
    {
        private readonly Dictionary<PeerHash, BattlefieldEntry> byPeer;

        public BattlefieldCollection()
        {
            byPeer = new Dictionary<PeerHash, BattlefieldEntry>();
        }

        public BattlefieldEntry GetOrCreate(PeerHash peer)
        {
            BattlefieldEntry entry;

            if (byPeer.TryGetValue(peer, out entry) == false)
            {
                entry = new BattlefieldEntry(peer);
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