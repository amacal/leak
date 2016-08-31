using Leak.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Telegraph
{
    public class TelegraphCollection
    {
        private readonly Dictionary<FileHash, Dictionary<string, TelegraphEntry>> byHash;

        public TelegraphCollection()
        {
            byHash = new Dictionary<FileHash, Dictionary<string, TelegraphEntry>>();
        }

        public TelegraphEntry GetOrCreate(FileHash hash, string tracker)
        {
            TelegraphEntry entry;
            Dictionary<string, TelegraphEntry> entries;

            if (byHash.TryGetValue(hash, out entries) == false)
            {
                entries = new Dictionary<string, TelegraphEntry>();
                byHash.Add(hash, entries);
            }

            if (entries.TryGetValue(tracker, out entry) == false)
            {
                entry = new TelegraphEntry(hash, tracker);
                entries.Add(tracker, entry);
            }

            return entry;
        }

        public IEnumerable<TelegraphEntry> Expired(DateTime now)
        {
            foreach (TelegraphEntry entry in byHash.Values.SelectMany(x => x.Values))
                if (entry.Next < now)
                    yield return entry;
        }
    }
}