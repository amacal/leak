using Leak.Common;

namespace Leak.Client.Notifications
{
    public class MetafileRequestedNotification : Notification
    {
        private readonly PeerHash hash;
        private readonly PieceInfo piece;

        public MetafileRequestedNotification(PeerHash hash, PieceInfo piece)
        {
            this.hash = hash;
            this.piece = piece;
        }

        public override NotificationType Type
        {
            get { return NotificationType.MetafileRequested; }
        }

        public override void Dispatch(NotificationVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override string ToString()
        {
            return $"Meta: requested; piece={Piece}";
        }

        public PeerHash Hash
        {
            get { return hash; }
        }

        public PieceInfo Piece
        {
            get { return piece; }
        }
    }
}