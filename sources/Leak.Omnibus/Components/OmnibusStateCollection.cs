using System.Collections.Generic;
using Leak.Common;
using Leak.Events;

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

            entry.State.IsLocalChokingRemote = data.IsLocalChokingRemote;
            entry.State.IsLocalInterestedInRemote = data.IsLocalInterestedInRemote;
            entry.State.IsRemoteChokingLocal = data.IsRemoteChokingLocal;
            entry.State.IsRemoteInterestedInLocal = data.IsRemoteInterestedInLocal;
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