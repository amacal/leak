using Leak.Core.Core;
using Leak.Core.Telegraph.Events;
using Leak.Core.Tracker;
using System;

namespace Leak.Core.Telegraph
{
    public class TelegraphTaskAnnounce : LeakTask<TelegraphContext>
    {
        private readonly TelegraphEntry entry;

        public TelegraphTaskAnnounce(TelegraphEntry entry)
        {
            this.entry = entry;
        }

        public void Execute(TelegraphContext context)
        {
            entry.Next = DateTime.Now.AddMinutes(30);

            context.Bus.Publish("tracker-announce-started", new TrackerAnnounceStarted
            {
                Hash = entry.Request.Hash,
                Peer = entry.Request.Peer,
                Tracker = entry.Tracker,
            });

            try
            {
                TrackerClient client = TrackerClientFactory.Create(entry.Tracker);
                TrackerAnnounce response = client.Announce(entry.Request);

                context.Bus.Publish("tracker-announce-completed", new TrackerAnnounceCompleted
                {
                    Hash = entry.Request.Hash,
                    Peer = entry.Request.Peer,
                    Tracker = entry.Tracker,
                    Peers = response.Peers,
                    Interval = response.Interval
                });

                entry.Next = DateTime.Now.Add(response.Interval);
            }
            catch (Exception ex)
            {
                context.Bus.Publish("tracker-announce-failed", new TrackerAnnounceFailed
                {
                    Hash = entry.Request.Hash,
                    Peer = entry.Request.Peer,
                    Tracker = entry.Tracker,
                    Reason = ex.Message
                });
            }
        }
    }
}