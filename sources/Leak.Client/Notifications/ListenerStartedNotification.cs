using System.Net;
using Leak.Common;

namespace Leak.Client.Notifications
{
    public class ListenerStartedNotification : Notification
    {
        private readonly PeerHash peer;
        private readonly int port;

        public ListenerStartedNotification(PeerHash peer, int port)
        {
            this.peer = peer;
            this.port = port;
        }

        public override NotificationType Type
        {
            get { return NotificationType.ListenerStarted; }
        }

        public override void Dispatch(NotificationVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override string ToString()
        {
            return $"Listener: started; endpoint={Endpoint}; peer={Peer}";
        }

        public PeerHash Peer
        {
            get { return peer; }
        }

        public PeerAddress Endpoint
        {
            get { return PeerAddress.Parse(new IPEndPoint(IPAddress.Any, port)); }
        }
    }
}