using Leak.Common;
using Leak.Events;
using System.Collections.Generic;

namespace Leak.Omnibus.Components
{
    public class OmnibusStateCollection
    {
        private readonly Dictionary<PeerHash, OmnibusStateEntry> byPeer;

        public OmnibusStateCollection()
        {
            byPeer = new Dictionary<PeerHash, OmnibusStateEntry>();
        }

        public void Handle(PeerChanged data)
        {
            OmnibusStateEntry entry;

            if (byPeer.TryGetValue(data.Peer, out entry) == false)
            {
                entry = new OmnibusStateEntry(data.Peer);
                byPeer.Add(data.Peer, entry);
            }

            entry.State = new PeerState(data.State);
        }

        public IEnumerable<PeerHash> Find(int ranking, int count)
        {
            foreach (OmnibusStateEntry state in byPeer.Values)
            {
                if (count == 0)
                    break;

                if (state.Ranking < ranking)
                    continue;

                if (state.State.IsRemoteChokingLocal)
                    continue;

                if (state.State.IsLocalInterestedInRemote == false)
                    continue;

                count--;
                yield return state.Peer;
            }
        }
    }
}