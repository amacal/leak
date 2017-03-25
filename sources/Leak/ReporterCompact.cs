using System;
using Leak.Client;
using Leak.Client.Notifications;
using Leak.Common;

namespace Leak
{
    public class ReporterCompact : NotificationVisitor, Reporter
    {
        private readonly ReporterPeers peers;
        private readonly ReporterResource resource;

        private Metainfo metainfo;
        private int completed;

        public ReporterCompact()
        {
            peers = new ReporterPeers();
            resource = new ReporterResource();
        }

        public bool Handle(Notification notification)
        {
            notification.Dispatch(this);

            Console.Write($"\rData: {peers}, completed {completed}, {resource}".PadRight(Console.WindowWidth - 2));

            return notification.Type != NotificationType.DataCompleted;
        }

        public override void Handle(PeerConnectedNotification notification)
        {
            peers.Handle(notification);
        }

        public override void Handle(PeerDisconnectedNotification notification)
        {
            peers.Handle(notification);
        }

        public override void Handle(StatusChangedNotification notification)
        {
            peers.Handle(notification);
        }

        public override void Handle(MetafileCompletedNotification notification)
        {
            Console.WriteLine();
            Console.WriteLine(notification);

            metainfo = notification.Metainfo;
        }

        public override void Handle(DataAllocatedNotification notification)
        {
            Console.WriteLine();
            Console.WriteLine(notification);
        }

        public override void Handle(DataVerifiedNotification notification)
        {
            completed = notification.Bitfield.Completed;

            Console.WriteLine();
            Console.WriteLine(notification);
        }

        public override void Handle(DataCompletedNotification notification)
        {
            Console.WriteLine();
            Console.WriteLine(notification);
        }

        public override void Handle(DataChangedNotification notification)
        {
            completed = notification.Completed;
        }

        public override void Handle(MemorySnapshotNotification notification)
        {
            resource.Handle(notification);
        }

        public override void Handle(PieceRejectedNotification notification)
        {
            Console.WriteLine();
            Console.WriteLine(notification);
        }
    }
}