using System;
using Leak.Client;
using Leak.Client.Notifications;
using Leak.Common;

namespace Leak.Reporting
{
    public class ReporterCompact : NotificationVisitor, Reporter
    {
        private readonly string command;
        private readonly ReporterPeers peers;
        private readonly ReporterResource resource;

        private Metainfo metainfo;
        private int completed;

        public ReporterCompact(string command)
        {
            this.command = command;

            peers = new ReporterPeers();
            resource = new ReporterResource();
        }

        public bool Handle(Notification notification)
        {
            notification.Dispatch(this);

            Console.Write($"\rData: {peers}, completed {completed}, {resource}".PadRight(Console.WindowWidth - 2));

            if (command == "download")
                return notification.Type != NotificationType.DataCompleted;

            return true;
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