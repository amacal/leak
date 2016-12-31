using System;
using System.Collections.Generic;
using Leak.Common;

namespace Leak.Leakage
{
    public class LeakCollection : IDisposable
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
                Hash = registrant.Hash,
                Destination = registrant.Destination
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

        public void Dispose()
        {
            foreach (LeakEntry entry in byHash.Values)
            {
                entry.Metafile.Dispose();
                entry.Spartan.Dispose();
            }

            byHash.Clear();
        }
    }
}