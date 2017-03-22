using Leak.Common;

namespace Leak.Client.Notifications
{
    public class PeerDisconnectedNotification : Notification
    {
        private readonly PeerHash peer;

        public PeerDisconnectedNotification(PeerHash peer)
        {
            this.peer = peer;
        }

        public override NotificationType Type
        {
            get { return NotificationType.PeerDisconnected; }
        }

        public override void Dispatch(NotificationVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override string ToString()
        {
            return $"Peer: disconnected; id={Peer}";
        }

        public PeerHash Peer
        {
            get { return peer; }
        }
    }
}