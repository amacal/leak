using System;
using System.Collections.Generic;
using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Peer.Coordinator
{
    public class CoordinatorCollection
    {
        private readonly Dictionary<PeerHash, CoordinatorEntry> byPeer;
        private readonly Dictionary<long, CoordinatorEntry> byConnection;

        public CoordinatorCollection()
        {
            byPeer = new Dictionary<PeerHash, CoordinatorEntry>();
            byConnection = new Dictionary<long, CoordinatorEntry>();
        }

        public CoordinatorEntry Add(NetworkConnection connection, Handshake handshake)
        {
            if (byPeer.ContainsKey(handshake.Remote))
                return null;

            CoordinatorEntry entry = new CoordinatorEntry
            {
                Identifier = connection.Identifier,
                Peer = handshake.Remote,
                Remote = connection.Remote,
                Direction = connection.Direction,
                State = new PeerState(),
                Timestamp = DateTime.Now
            };

            byPeer.Add(entry.Peer, entry);
            byConnection.Add(entry.Identifier, entry);

            return entry;
        }

        public CoordinatorEntry Remove(NetworkConnection connection)
        {
            CoordinatorEntry entry;
            long identifier = connection.Identifier;

            if (byConnection.TryGetValue(identifier, out entry) == false)
                return null;

            byPeer.Remove(entry.Peer);
            byConnection.Remove(entry.Identifier);

            return entry;
        }

        public CoordinatorEntry Find(PeerHash peer)
        {
            CoordinatorEntry entry;
            byPeer.TryGetValue(peer, out entry);
            return entry;
        }

        public IEnumerable<CoordinatorEntry> All()
        {
            return byPeer.Values;
        }
    }
}