using Leak.Common;

namespace Leak.Client.Notifications
{
    public class PieceCompletedNotification : Notification
    {
        private readonly FileHash hash;
        private readonly PieceInfo piece;

        public PieceCompletedNotification(FileHash hash, PieceInfo piece)
        {
            this.hash = hash;
            this.piece = piece;
        }

        public override NotificationType Type
        {
            get { return NotificationType.PieceCompleted; }
        }

        public override void Dispatch(NotificationVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override string ToString()
        {
            return $"Piece: completed; piece={Piece}";
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