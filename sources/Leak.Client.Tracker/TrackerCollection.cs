using System.Collections.Generic;
using Leak.Common;

namespace Leak.Client.Tracker
{
    public class TrackerCollection
    {
        private readonly TrackerLogger logger;
        private readonly Dictionary<FileHash, TrackerEntry> items;

        public TrackerCollection(TrackerLogger logger)
        {
            this.logger = logger;
            items = new Dictionary<FileHash, TrackerEntry>();
        }

        public TrackerEntry Add(FileHash hash)
        {
            TrackerEntry entry = new TrackerEntry
            {
                Hash = hash
            };

            lock (items)
            {
                logger?.Info($"registering new entry '{hash}'");
                items.Add(hash, entry);
            }

            return entry;
        }

        public TrackerEntry Find(FileHash hash)
        {
            TrackerEntry entry;

            lock (items)
            {
                logger?.Info($"looking for entry '{hash}");
                items.TryGetValue(hash, out entry);
            }

            return entry;
        }

        public void Remove(FileHash hash)
        {
            lock (items)
            {
                logger?.Info($"removing entry '{hash}");
                items.Remove(hash);
            }
        }
    }
}