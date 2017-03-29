using Leak.Common;

namespace Leak.Client.Notifications
{
    public class MetafileRequestedNotification : Notification
    {
        private readonly FileHash hash;
        private readonly PieceInfo piece;

        public MetafileRequestedNotification(FileHash hash, PieceInfo piece)
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

        public FileHash Hash
        {
            get { return hash; }
        }

        public PieceInfo Piece
        {
            get { return piece; }
        }
    }
}