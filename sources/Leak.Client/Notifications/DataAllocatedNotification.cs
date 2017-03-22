using Leak.Common;

namespace Leak.Client.Notifications
{
    public class DataAllocatedNotification : Notification
    {
        private readonly FileHash hash;
        private readonly string directory;

        public DataAllocatedNotification(FileHash hash, string directory)
        {
            this.hash = hash;
            this.directory = directory;
        }

        public override NotificationType Type
        {
            get { return NotificationType.DataAllocated; }
        }

        public override void Dispatch(NotificationVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override string ToString()
        {
            return $"Data: allocated; directory={Directory}";
        }

        public FileHash Hash
        {
            get { return hash; }
        }

        public string Directory
        {
            get { return directory; }
        }
    }
}