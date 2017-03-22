using Leak.Common;

namespace Leak.Client.Notifications
{
    public class MetafileMeasuredNotification : Notification
    {
        private readonly FileHash hash;
        private readonly Size size;

        public MetafileMeasuredNotification(FileHash hash, Size size)
        {
            this.hash = hash;
            this.size = size;
        }

        public override NotificationType Type
        {
            get { return NotificationType.MetafileMeasured; }
        }

        public override void Dispatch(NotificationVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override string ToString()
        {
            return $"Meta: measured; hash={Hash}; size={Size}";
        }

        public FileHash Hash
        {
            get { return hash; }
        }

        public Size Size
        {
            get { return size; }
        }
    }
}