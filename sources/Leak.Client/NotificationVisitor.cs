using Leak.Client.Notifications;

namespace Leak.Client
{
    public abstract class NotificationVisitor
    {
        public virtual void Handle(PeerConnectedNotification notification)
        {
        }

        public virtual void Handle(PeerDisconnectedNotification notification)
        {
        }

        public virtual void Handle(PeerRejectedNotification notification)
        {
        }

        public virtual void Handle(BitfieldChangedNotification notification)
        {
        }

        public virtual void Handle(StatusChangedNotification notification)
        {
        }

        public virtual void Handle(MetafileMeasuredNotification notification)
        {
        }

        public virtual void Handle(MetafileRequestedNotification notification)
        {
        }

        public virtual void Handle(MetafileReceivedNotification notification)
        {
        }

        public virtual void Handle(MetafileCompletedNotification notification)
        {
        }

        public virtual void Handle(DataAllocatedNotification notification)
        {
        }

        public virtual void Handle(DataVerifiedNotification notification)
        {
        }

        public virtual void Handle(DataChangedNotification notification)
        {
        }

        public virtual void Handle(DataCompletedNotification notification)
        {
        }

        public virtual void Handle(PieceCompletedNotification notification)
        {
        }

        public virtual void Handle(PieceRejectedNotification notification)
        {
        }

        public virtual void Handle(MemorySnapshotNotification notification)
        {
        }

        public virtual void Handle(ListenerStartedNotification notification)
        {
        }

        public virtual void Handle(ListenerFailedNotification notification)
        {
        }

        public virtual void Handle(TrackerAnnouncedNotification notification)
        {
        }
    }
}