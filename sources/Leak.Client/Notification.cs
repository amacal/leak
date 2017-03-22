namespace Leak.Client
{
    public abstract class Notification
    {
        public abstract NotificationType Type { get; }

        public abstract void Dispatch(NotificationVisitor visitor);
    }
}