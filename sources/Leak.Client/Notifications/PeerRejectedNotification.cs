using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Client.Notifications
{
    public class PeerRejectedNotification : Notification
    {
        private readonly NetworkAddress remote;

        public PeerRejectedNotification(NetworkAddress remote)
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

        public NetworkAddress Remote
        {
            get { return remote; }
        }
    }
}