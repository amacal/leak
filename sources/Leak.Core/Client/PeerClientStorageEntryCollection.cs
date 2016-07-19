using Leak.Core.Common;
using System.Collections.Generic;

namespace Leak.Core.Client
{
    public class PeerClientStorageEntryCollection
    {
        private readonly Dictionary<FileHash, PeerClientStorageEntry> byHash;
        private readonly Dictionary<PeerHash, PeerClientStorageEntry> byPeer;

        public PeerClientStorageEntryCollection()
        {
            this.byHash = new Dictionary<FileHash, PeerClientStorageEntry>();
            this.byPeer = new Dictionary<PeerHash, PeerClientStorageEntry>();
        }

        public void Add(FileHash hash, PeerClientStorageEntry entry)
        {
            byHash.Add(hash, entry);
        }

        public void AddPeer(FileHash hash, PeerHash peer)
        {
            byPeer.Add(peer, byHash[hash]);
        }

        public PeerClientStorageEntry ByHash(FileHash hash)
        {
            return byHash[hash];
        }

        public PeerClientStorageEntry ByPeer(PeerHash peer)
        {
            return byPeer[peer];
        }
    }
}