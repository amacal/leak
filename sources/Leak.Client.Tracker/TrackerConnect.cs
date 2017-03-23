using System;
using System.Threading.Tasks;
using Leak.Client.Notifications;
using Leak.Common;
using Leak.Completion;
using Leak.Tasks;
using Leak.Tracker.Get;
using Leak.Tracker.Get.Events;

namespace Leak.Client.Tracker
{
    internal class TrackerConnect
    {
        public CompletionWorker Worker { get; set; }
        public PipelineService Pipeline { get; set; }

        public string Tracker { get; set; }
        public PeerHash Localhost { get; set; }

        public NotificationCollection Notifications { get; set; }
        public TaskCompletionSource<TrackerSession> Completion { get; set; }

        public TrackerGetService TrackerGet { get; set; }

        public void Start()
        {
            StartTrackerGet();

            Completion.SetResult(new TrackerSession(this));
        }

        public void Announce(FileHash[] hashes)
        {
            foreach (FileHash hash in hashes)
            {
                TrackerGet?.Register(new TrackerGetRegistrant
                {
                    Hash = hash,
                    Address = new Uri(Tracker, UriKind.Absolute)
                });
            }
        }

        protected virtual void StartTrackerGet()
        {
            TrackerGetHooks hooks = new TrackerGetHooks
            {
                OnAnnounced = OnTrackerAnnounced
            };

            TrackerGet =
                new TrackerGetBuilder()
                    .WithPipeline(Pipeline)
                    .WithWorker(Worker)
                    .WithPeer(Localhost)
                    .Build(hooks);

            TrackerGet.Start();
        }

        private void OnTrackerAnnounced(TrackerAnnounced data)
        {
            Notifications.Enqueue(new TrackerAnnouncedNotification(data.Hash, data.Address.ToString(), data.Peers));
        }
    }
}