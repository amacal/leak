using System.Collections.Generic;
using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Peer.Sender
{
    public class SenderCollection
    {
        private readonly Dictionary<PeerHash, NetworkConnection> connections;

        public SenderCollection()
        {
            connections = new Dictionary<PeerHash, NetworkConnection>();
        }

        public void Add(PeerHash peer, NetworkConnection connection)
        {
            if (connections.ContainsKey(peer) == false)
            {
                connections.Add(peer, connection);
            }
        }

        public void Remove(PeerHash peer)
        {
            connections.Remove(peer);
        }

        public NetworkConnection Find(PeerHash peer)
        {
            NetworkConnection connection;

            connections.TryGetValue(peer, out connection);
            return connection;
        }
    }
}