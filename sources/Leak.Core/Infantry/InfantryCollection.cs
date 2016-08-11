using System.Collections.Generic;
using System.Linq;
using Leak.Core.Common;

namespace Leak.Core.Infantry
{
    public class InfantryCollection
    {
        private readonly Dictionary<PeerHash, InfantryEntry> byPeer;
        private readonly Dictionary<FileHash, HashSet<InfantryEntry>> byHash;

        public InfantryCollection()
        {
            byPeer = new Dictionary<PeerHash, InfantryEntry>();
            byHash = new Dictionary<FileHash, HashSet<InfantryEntry>>();
        }

        public InfantryEntry Get(PeerHash peer)
        {
            InfantryEntry entry;

            if (byPeer.TryGetValue(peer, out entry))
            {
                return entry;
            }

            return null;
        }

        public InfantryEntry GetOrCreate(PeerHash peer)
        {
            InfantryEntry entry;

            if (byPeer.TryGetValue(peer, out entry) == false)
            {
                entry = new InfantryEntry(peer);
                byPeer.Add(peer, entry);
            }

            return entry;
        }

        public void Register(FileHash hash, InfantryEntry entry)
        {
            HashSet<InfantryEntry> entries;

            if (byHash.TryGetValue(hash, out entries) == false)
            {
                entries = new HashSet<InfantryEntry>();
                byHash.Add(hash, entries);
            }

            entry.Hash = hash;
            entries.Add(entry);
        }

        public void Remove(PeerHash peer)
        {
            InfantryEntry entry;
            HashSet<InfantryEntry> entries;

            if (byPeer.TryGetValue(peer, out entry))
            {
                byPeer.Remove(peer);
            }

            if (entry?.Hash != null && byHash.TryGetValue(entry.Hash, out entries))
            {
                entries.Remove(entry);
            }
        }

        public IEnumerable<InfantryEntry> Get(FileHash hash)
        {
            HashSet<InfantryEntry> entries;

            if (byHash.TryGetValue(hash, out entries))
            {
                return entries;
            }

            return Enumerable.Empty<InfantryEntry>();
        }
    }
}