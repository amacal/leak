using System.Collections.Generic;
using Leak.Common;

namespace Leak.Core.Leakage
{
    public class LeakCollection
    {
        private readonly Dictionary<FileHash, LeakEntry> byHash;

        public LeakCollection()
        {
            byHash = new Dictionary<FileHash, LeakEntry>();
        }

        public LeakEntry Register(LeakRegistrant registrant)
        {
            LeakEntry entry = new LeakEntry
            {
                Hash = registrant.Hash
            };

            byHash.Add(entry.Hash, entry);
            return entry;
        }

        public LeakEntry Find(FileHash hash)
        {
            LeakEntry entry;
            byHash.TryGetValue(hash, out entry);
            return entry;
        }
    }
}