using Leak.Common;

namespace Leak.Client.Notifications
{
    public class StatusChangedNotification : Notification
    {
        private readonly PeerHash peer;
        private readonly PeerState state;

        public StatusChangedNotification(PeerHash peer, PeerState state)
        {
            this.peer = peer;
            this.state = state;
        }

        public override NotificationType Type
        {
            get { return NotificationType.PeerStatusChanged; }
        }

        public override void Dispatch(NotificationVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override string ToString()
        {
            return $"Peer: id={Peer}; status={State}";
        }

        public PeerHash Peer
        {
            get { return peer; }
        }

        public PeerState State
        {
            get { return state; }
        }
    }
}