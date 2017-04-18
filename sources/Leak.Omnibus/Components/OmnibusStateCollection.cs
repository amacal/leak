using System.Collections.Generic;
using System.Linq;
using Leak.Common;
using Leak.Events;
using Leak.Peer.Coordinator.Events;

namespace Leak.Data.Map.Components
{
    public class OmnibusStateCollection
    {
        private readonly Dictionary<PeerHash, OmnibusStateEntry> byPeer;

        public OmnibusStateCollection()
        {
            byPeer = new Dictionary<PeerHash, OmnibusStateEntry>();
        }

        public void Handle(PeerConnected data)
        {
            OmnibusStateEntry entry;

            if (byPeer.TryGetValue(data.Peer, out entry) == false)
            {
                entry = new OmnibusStateEntry(data.Peer);
                byPeer.Add(data.Peer, entry);
            }

            entry.State = new PeerState();
        }

        public void Handle(PeerDisconnected data)
        {
            byPeer.Remove(data.Peer);
        }

        public void Handle(StatusChanged data)
        {
            OmnibusStateEntry entry;

            if (byPeer.TryGetValue(data.Peer, out entry) == false)
            {
                entry = new OmnibusStateEntry(data.Peer);
                byPeer.Add(data.Peer, entry);
            }

            entry.State = new PeerState(data.State);
        }

        public IEnumerable<PeerHash> All()
        {
            return byPeer.Keys;
        }

        public OmnibusStateEntry ByPeer(PeerHash peer)
        {
            OmnibusStateEntry entry;
            byPeer.TryGetValue(peer, out entry);
            return entry;
        }

        public IEnumerable<PeerHash> Find(int ranking, int count)
        {
            foreach (OmnibusStateEntry state in byPeer.Values.OrderByDescending(x => x.Ranking))
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