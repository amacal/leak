using Leak.Common;

namespace Leak.Client.Notifications
{
    public class DataCompletedNotification : Notification
    {
        private readonly FileHash hash;

        public DataCompletedNotification(FileHash hash)
        {
            this.hash = hash;
        }

        public override NotificationType Type
        {
            get { return NotificationType.DataCompleted; }
        }

        public override void Dispatch(NotificationVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override string ToString()
        {
            return "Data: completed";
        }

        public FileHash Hash
        {
            get { return hash; }
        }
    }
}