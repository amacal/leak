using Leak.Common;

namespace Leak.Client.Notifications
{
    public class PieceRejectedNotification : Notification
    {
        private readonly FileHash hash;
        private readonly PieceInfo piece;

        public PieceRejectedNotification(FileHash hash, PieceInfo piece)
        {
            this.hash = hash;
            this.piece = piece;
        }

        public override NotificationType Type
        {
            get { return NotificationType.PieceRejected; }
        }

        public override void Dispatch(NotificationVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override string ToString()
        {
            return $"Piece: rejected; piece={Piece}";
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