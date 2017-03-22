using Leak.Common;

namespace Leak.Client.Notifications
{
    public class MetafileReceivedNotification : Notification
    {
        private readonly FileHash hash;
        private readonly PieceInfo piece;

        public MetafileReceivedNotification(FileHash hash, PieceInfo piece)
        {
            this.hash = hash;
            this.piece = piece;
        }

        public override NotificationType Type
        {
            get { return NotificationType.MetafileReceived; }
        }

        public override void Dispatch(NotificationVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override string ToString()
        {
            return $"Meta: received; piece={Piece}";
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