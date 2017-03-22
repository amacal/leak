using Leak.Common;

namespace Leak.Client.Notifications
{
    public class ListenerFailedNotification : Notification
    {
        private readonly PeerHash peer;
        private readonly string reason;

        public ListenerFailedNotification(PeerHash peer, string reason)
        {
            this.peer = peer;
            this.reason = reason;
        }

        public override NotificationType Type
        {
            get { return NotificationType.ListenerFailed; }
        }

        public override void Dispatch(NotificationVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override string ToString()
        {
            return $"Listener: failed; reason='{Reason}'";
        }

        public PeerHash Peer
        {
            get { return peer; }
        }

        public string Reason
        {
            get { return reason; }
        }
    }
}