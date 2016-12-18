using System.Collections.Generic;
using Leak.Common;
using Leak.Events;

namespace Leak.Omnibus.Components
{
    public class OmnibusStateCollection
    {
        private readonly Dictionary<PeerHash, OmnibusState> byPeer;

        public OmnibusStateCollection()
        {
            byPeer = new Dictionary<PeerHash, OmnibusState>();
        }

        public void Handle(PeerChanged data)
        {
            OmnibusState state;

            if (byPeer.TryGetValue(data.Peer, out state) == false)
            {
                state = new OmnibusState { Peer = data.Peer };
                byPeer.Add(data.Peer, state);
            }

            state.IsLocalChokingRemote = data.IsLocalChokingRemote;
            state.IsLocalInterestedInRemote = data.IsLocalInterestedInRemote;
            state.IsRemoteChokingLocal = data.IsRemoteChokingLocal;
            state.IsRemoteInterestedInLocal = data.IsRemoteInterestedInLocal;
        }

        public IEnumerable<PeerHash> Find(int ranking, int count)
        {
            foreach (OmnibusState state in byPeer.Values)
            {
                if (count == 0)
                    break;

                if (state.Ranking < ranking)
                    continue;

                if (state.IsRemoteChokingLocal)
                    continue;

                if (state.IsLocalInterestedInRemote == false)
                    continue;

                count--;
                yield return state.Peer;
            }
        }
    }
}