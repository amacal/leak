using System;
using System.Collections.Generic;

namespace Leak.Tracker.Get
{
    public class TrackerGetCollection
    {
        private readonly List<TrackerGetEntry> entries;

        public TrackerGetCollection()
        {
            entries = new List<TrackerGetEntry>();
        }

        public void Add(TrackerGetRegistrant registrant)
        {
            entries.Add(new TrackerGetEntry
            {
                Request = registrant,
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