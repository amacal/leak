using System;
using System.Collections.Generic;
using Leak.Client;
using Leak.Client.Notifications;
using Leak.Common;

namespace Leak
{
    public class ReporterCompact : NotificationVisitor, Reporter
    {
        private readonly HashSet<PeerHash> peers;
        private readonly HashSet<PeerHash> seeds;

        private Metainfo metainfo;
        private int completed;
        private Size memory;

        public ReporterCompact()
        {
            peers = new HashSet<PeerHash>();
            seeds = new HashSet<PeerHash>();
            memory = new Size(0);
        }

        public bool Handle(Notification notification)
        {
            notification.Dispatch(this);

            Console.Write($"\rData: peers {peers.Count}, seeds {seeds.Count}, completed {completed}, memory {memory}".PadRight(80));

            return notification.Type != NotificationType.DataCompleted;
        }

        public override void Handle(PeerConnectedNotification notification)
        {
            peers.Add(notification.Peer);
        }

        public override void Handle(PeerDisconnectedNotification notification)
        {
            peers.Remove(notification.Peer);
            seeds.Remove(notification.Peer);
        }

        public override void Handle(StatusChangedNotification notification)
        {
            if (notification.State.IsRemoteChokingLocal == false)
                seeds.Add(notification.Peer);
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
            memory = notification.Allocation;
        }

        public override void Handle(PieceRejectedNotification notification)
        {
            Console.WriteLine();
            Console.WriteLine(notification);
        }
    }
}