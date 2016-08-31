using Leak.Core.Common;
using Leak.Core.Tracker;
using System;

namespace Leak.Core.Telegraph
{
    public class TelegraphEntry
    {
        private readonly FileHash hash;
        private readonly string tracker;

        public TelegraphEntry(FileHash hash, string tracker)
        {
            this.hash = hash;
            this.tracker = tracker;
        }

        public FileHash Hash
        {
            get { return hash; }
        }

        public string Tracker
        {
            get { return tracker; }
        }

        public TrackerRequest Request { get; set; }

        public DateTime Next { get; set; }
    }
}