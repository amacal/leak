using Leak.Common;

namespace Leak.Client.Notifications
{
    public class MemorySnapshotNotification : Notification
    {
        private readonly Size allocation;

        public MemorySnapshotNotification(Size allocation)
        {
            this.allocation = allocation;
        }

        public override NotificationType Type
        {
            get { return NotificationType.MemorySnapshot; }
        }

        public override void Dispatch(NotificationVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override string ToString()
        {
            return $"Memory: allocation={Allocation}";
        }

        public Size Allocation
        {
            get { return allocation; }
        }
    }
}