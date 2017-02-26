using System;
using Leak.Tasks;

namespace Leak.Tracker.Get
{
    public class TrackerGetNextTask : LeakTask<TrackerGetContext>
    {
        public void Execute(TrackerGetContext context)
        {
            DateTime now = DateTime.Now;
            TrackerGetFactory factory = new TrackerGetFactory(context);

            foreach (TrackerGetEntry entry in context.Collection.Find(now))
            {
                entry.Next = now + TimeSpan.FromMinutes(15);
                factory.Create(entry.Address).Announce(entry.Hash);
            }
        }
    }
}