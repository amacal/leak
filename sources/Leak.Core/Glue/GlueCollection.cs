using Leak.Core.Negotiator;
using Leak.Core.Network;
using System;
using System.Collections.Generic;
using Leak.Common;

namespace Leak.Core.Glue
{
    public class GlueCollection
    {
        private readonly Dictionary<PeerHash, GlueEntry> byPeer;
        private readonly Dictionary<long, GlueEntry> byConnection;

        public GlueCollection()
        {
            byPeer = new Dictionary<PeerHash, GlueEntry>();
            byConnection = new Dictionary<long, GlueEntry>();
        }

        public GlueEntry Add(NetworkConnection connection, Handshake handshake)
        {
            GlueEntry entry = new GlueEntry
            {
                Identifier = connection.Identifier,
                Peer = handshake.Remote,
                Remote = connection.Remote,
                Direction = connection.Direction,
                State = GlueState.Default,
                Timestamp = DateTime.Now
            };

            byPeer.Add(entry.Peer, entry);
            byConnection.Add(entry.Identifier, entry);

            return entry;
        }

        public GlueEntry Remove(NetworkConnection connection)
        {
            GlueEntry entry = byConnection[connection.Identifier];

            byPeer.Remove(entry.Peer);
            byConnection.Remove(entry.Identifier);

            return entry;
        }

        public GlueEntry Find(PeerHash peer)
        {
            GlueEntry entry;
            byPeer.TryGetValue(peer, out entry);
            return entry;
        }

        public IEnumerable<GlueEntry> All()
        {
            return byPeer.Values;
        }
    }
}