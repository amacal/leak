using Leak.Core.Common;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Client
{
    public class PeerClientStorageEntryCollection
    {
        private readonly object synchronized;

        private readonly Dictionary<FileHash, PeerClientStorageEntry> byHash;
        private readonly Dictionary<PeerHash, PeerClientStorageEntry> byPeer;
        private readonly Dictionary<string, PeerClientStorageEntry> byRemote;

        public PeerClientStorageEntryCollection()
        {
            this.synchronized = new object();

            this.byHash = new Dictionary<FileHash, PeerClientStorageEntry>();
            this.byPeer = new Dictionary<PeerHash, PeerClientStorageEntry>();
            this.byRemote = new Dictionary<string, PeerClientStorageEntry>();
        }

        public void Add(FileHash hash, PeerClientStorageEntry entry)
        {
            lock (synchronized)
            {
                byHash.Add(hash, entry);
            }
        }

        public bool AddEndpoint(PeerEndpoint endpoint)
        {
            lock (synchronized)
            {
                if (byPeer.ContainsKey(endpoint.Peer))
                    return false;

                if (byRemote.ContainsKey(endpoint.Remote))
                    return false;

                byPeer.Add(endpoint.Peer, byHash[endpoint.Hash]);
                byRemote.Add(endpoint.Remote, byHash[endpoint.Hash]);
                byHash[endpoint.Hash].Peers.Add(endpoint.Peer);

                return true;
            }
        }

        public bool Contains(string remote)
        {
            lock (synchronized)
            {
                return byRemote.ContainsKey(remote);
            }
        }

        public PeerClientStorageEntry ByHash(FileHash hash)
        {
            lock (synchronized)
            {
                return byHash[hash];
            }
        }

        public PeerClientStorageEntry ByPeer(PeerHash peer)
        {
            lock (synchronized)
            {
                return byPeer[peer];
            }
        }

        public FileHash Hash()
        {
            lock (synchronized)
            {
                return byHash.Keys.Single();
            }
        }
    }
}