using Leak.Common;

namespace Leak.Client.Notifications
{
    public class PeerRejectedNotification : Notification
    {
        private readonly PeerAddress remote;

        public PeerRejectedNotification(PeerAddress remote)
        {
            this.remote = remote;
        }

        public override NotificationType Type
        {
            get { return NotificationType.PeerRejected; }
        }

        public override void Dispatch(NotificationVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override string ToString()
        {
            return $"Peer: rejected; endpoint={Remote}";
        }

        public PeerAddress Remote
        {
            get { return remote; }
        }
    }
}