using Leak.Core.Common;
using System.Collections.Generic;

namespace Leak.Core.Collector
{
    public class PeerCollectorStorageEntryCollection
    {
        private readonly Dictionary<string, PeerCollectorStorageEntry> byRemote;
        private readonly Dictionary<PeerHash, PeerCollectorStorageEntry> byPeer;

        public PeerCollectorStorageEntryCollection()
        {
            byRemote = new Dictionary<string, PeerCollectorStorageEntry>();
            byPeer = new Dictionary<PeerHash, PeerCollectorStorageEntry>();
        }

        public PeerCollectorStorageEntry CreateByRemote(string remote)
        {
            PeerCollectorStorageEntry entry;

            if (byRemote.TryGetValue(remote, out entry) == false)
            {
                entry = new PeerCollectorStorageEntry();
                byRemote.Add(remote, entry);
            }

            return entry;
        }

        public PeerCollectorStorageEntry FindByRemote(string remote)
        {
            PeerCollectorStorageEntry entry;
            byRemote.TryGetValue(remote, out entry);
            return entry;
        }

        public PeerCollectorStorageEntry FindByPeer(PeerHash peer)
        {
            PeerCollectorStorageEntry entry;
            byPeer.TryGetValue(peer, out entry);
            return entry;
        }

        public void AddByPeer(PeerHash peer, PeerCollectorStorageEntry entry)
        {
            byPeer.Add(peer, entry);
        }

        public void AddByHash(FileHash hash, PeerCollectorStorageEntry entry)
        {
        }

        public void RemoveByRemote(string remote)
        {
            byRemote.Remove(remote);
        }

        public void RemoveByPeer(PeerHash peer)
        {
            if (peer != null)
            {
                byPeer.Remove(peer);
            }
        }
    }
}