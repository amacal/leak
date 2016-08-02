using Leak.Core.Common;
using System.Collections.Generic;

namespace Leak.Core.Bouncer
{
    public class PeerBouncerStorageEntryCollection
    {
        private readonly Dictionary<long, PeerBouncerStorageEntry> byIdentifier;
        private readonly Dictionary<string, PeerBouncerStorageEntry> byRemote;
        private readonly Dictionary<PeerHash, PeerBouncerStorageEntry> byPeer;

        public PeerBouncerStorageEntryCollection()
        {
            byIdentifier = new Dictionary<long, PeerBouncerStorageEntry>();
            byRemote = new Dictionary<string, PeerBouncerStorageEntry>();
            byPeer = new Dictionary<PeerHash, PeerBouncerStorageEntry>();
        }

        public PeerBouncerStorageEntry FindOrCreateByIdentifier(long identifier)
        {
            PeerBouncerStorageEntry entry;

            if (byIdentifier.TryGetValue(identifier, out entry) == false)
            {
                entry = new PeerBouncerStorageEntry();
                byIdentifier.Add(identifier, entry);
            }

            return entry;
        }

        public PeerBouncerStorageEntry FindOrDefaultByRemote(string remote)
        {
            PeerBouncerStorageEntry entry;
            byRemote.TryGetValue(remote, out entry);
            return entry;
        }

        public PeerBouncerStorageEntry FindOrDefaultByPeer(PeerHash peer)
        {
            PeerBouncerStorageEntry entry;
            byPeer.TryGetValue(peer, out entry);
            return entry;
        }

        public void AddByRemote(string remote, PeerBouncerStorageEntry entry)
        {
            byRemote.Add(remote, entry);
        }

        public void AddByPeer(PeerHash peer, PeerBouncerStorageEntry entry)
        {
            byPeer.Add(peer, entry);
        }

        public void RemoveByIdentifier(long identifier)
        {
            byIdentifier.Remove(identifier);
        }

        public void RemoveByRemote(string remote)
        {
            byRemote.Remove(remote);
        }

        public void RemoveByPeer(PeerHash peer)
        {
            byPeer.Remove(peer);
        }
    }
}