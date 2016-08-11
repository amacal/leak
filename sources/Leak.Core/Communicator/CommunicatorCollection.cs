using Leak.Core.Common;
using System.Collections.Generic;

namespace Leak.Core.Communicator
{
    public class CommunicatorCollection
    {
        private readonly Dictionary<PeerHash, CommunicatorEntry> byPeer;

        public CommunicatorCollection()
        {
            byPeer = new Dictionary<PeerHash, CommunicatorEntry>();
        }

        public CommunicatorEntry GetOrCreate(PeerHash peer)
        {
            CommunicatorEntry entry;

            if (byPeer.TryGetValue(peer, out entry) == false)
            {
                entry = new CommunicatorEntry();
                byPeer.Add(peer, entry);
            }

            return entry;
        }

        public void Remove(PeerHash peer)
        {
            CommunicatorEntry entry;

            if (byPeer.TryGetValue(peer, out entry))
            {
                entry.External = null;
                byPeer.Remove(peer);
            }
        }
    }
}