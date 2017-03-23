using System;
using System.Text;
using Leak.Common;

namespace Leak.Client.Notifications
{
    public class TrackerAnnouncedNotification : Notification
    {
        private readonly FileHash hash;
        private readonly string tracker;
        private readonly PeerAddress[] peers;

        public TrackerAnnouncedNotification(FileHash hash, string tracker, PeerAddress[] peers)
        {
            this.hash = hash;
            this.tracker = tracker;
            this.peers = peers;
        }

        public override NotificationType Type
        {
            get { return NotificationType.TrackerAnnounced; }
        }

        public override void Dispatch(NotificationVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"Tracker: announced; hash={Hash}; uri={Tracker};");

            foreach (PeerAddress peer in Peers)
            {
                builder.AppendLine($"Tracker: peer={peer}");
            }

            return builder.ToString().TrimEnd(Environment.NewLine.ToCharArray());
        }

        public FileHash Hash
        {
            get { return hash; }
        }

        public string Tracker
        {
            get { return tracker; }
        }

        public PeerAddress[] Peers
        {
            get { return peers; }
        }
    }
}