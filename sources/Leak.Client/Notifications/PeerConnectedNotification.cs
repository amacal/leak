using Leak.Common;

namespace Leak.Client.Notifications
{
    public class PeerConnectedNotification : Notification
    {
        private readonly PeerHash peer;

        public PeerConnectedNotification(PeerHash peer)
        {
            this.peer = peer;
        }

        public override NotificationType Type
        {
            get { return NotificationType.PeerConnected; }
        }

        public override void Dispatch(NotificationVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override string ToString()
        {
            return $"Peer: connected; id={Peer}";
        }

        public PeerHash Peer
        {
            get { return peer; }
        }
    }
}