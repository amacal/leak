using Leak.Common;

namespace Leak.Client.Notifications
{
    public class DataChangedNotification : Notification
    {
        private readonly FileHash hash;
        private readonly int completed;

        public DataChangedNotification(FileHash hash, int completed)
        {
            this.hash = hash;
            this.completed = completed;
        }

        public override NotificationType Type
        {
            get { return NotificationType.DataChanged; }
        }

        public override void Dispatch(NotificationVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override string ToString()
        {
            return $"Data: changed; completed={Completed}";
        }

        public FileHash Hash
        {
            get { return hash; }
        }

        public int Completed
        {
            get { return completed; }
        }
    }
}