using Leak.Common;

namespace Leak.Client.Notifications
{
    public class DataVerifiedNotification : Notification
    {
        private readonly FileHash hash;
        private readonly Bitfield bitfield;

        public DataVerifiedNotification(FileHash hash, Bitfield bitfield)
        {
            this.hash = hash;
            this.bitfield = bitfield;
        }

        public override NotificationType Type
        {
            get { return NotificationType.DataVerified; }
        }

        public override void Dispatch(NotificationVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override string ToString()
        {
            return $"Data: verified; bitfield={Bitfield.Completed}/{Bitfield.Length}";
        }

        public FileHash Hash
        {
            get { return hash; }
        }

        public Bitfield Bitfield
        {
            get { return bitfield; }
        }
    }
}