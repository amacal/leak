using Leak.Core.Common;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Infantry
{
    public class InfantryCollection
    {
        private readonly Dictionary<PeerHash, InfantryEntry> byPeer;
        private readonly Dictionary<FileHash, HashSet<InfantryEntry>> byHash;
        private readonly Dictionary<PeerAddress, InfantryEntry> byAddress;

        public InfantryCollection()
        {
            byPeer = new Dictionary<PeerHash, InfantryEntry>();
            byHash = new Dictionary<FileHash, HashSet<InfantryEntry>>();
            byAddress = new Dictionary<PeerAddress, InfantryEntry>();
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

        public InfantryEntry Get(PeerAddress address)
        {
            InfantryEntry entry;
            byAddress.TryGetValue(address, out entry);
            return entry;
        }

        public void Register(InfantryEntry entry)
        {
            HashSet<InfantryEntry> entries;

            if (byHash.TryGetValue(entry.Session.Hash, out entries) == false)
            {
                entries = new HashSet<InfantryEntry>();
                byHash.Add(entry.Session.Hash, entries);
            }

            byAddress.Add(entry.Address, entry);
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

            if (entry?.Session.Hash != null && byHash.TryGetValue(entry.Session.Hash, out entries))
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