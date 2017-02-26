using System;
using System.Collections.Generic;
using Leak.Common;

namespace Leak.Tracker.Get
{
    public class TrackerGetCollection
    {
        private readonly List<TrackerGetEntry> entries;

        public TrackerGetCollection()
        {
            entries = new List<TrackerGetEntry>();
        }

        public void Add(Uri address, FileHash hash)
        {
            entries.Add(new TrackerGetEntry
            {
                Hash = hash,
                Address = address,
                Next = DateTime.Now
            });
        }

        public IEnumerable<TrackerGetEntry> Find(DateTime now)
        {
            foreach (TrackerGetEntry entry in entries)
            {
                if (entry.Next < now)
                {
                    yield return entry;
                }
            }
        }
    }
}