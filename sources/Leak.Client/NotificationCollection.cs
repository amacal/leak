using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Leak.Client
{
    public class NotificationCollection
    {
        private readonly ConcurrentQueue<TaskCompletionSource<Notification>> completions;
        private readonly ConcurrentQueue<Notification> notifications;

        public NotificationCollection()
        {
            completions = new ConcurrentQueue<TaskCompletionSource<Notification>>();
            notifications = new ConcurrentQueue<Notification>();
        }

        public Task<Notification> NextAsync()
        {
            lock (this)
            {
                Notification notification;
                TaskCompletionSource<Notification> completion;

                if (notifications.TryDequeue(out notification))
                {
                    completion = new TaskCompletionSource<Notification>();
                    completion.SetResult(notification);

                    return completion.Task;
                }

                completion = new TaskCompletionSource<Notification>();
                completions.Enqueue(completion);

                return completion.Task;
            }
        }

        public void Enqueue(Notification notification)
        {
            lock (this)
            {
                TaskCompletionSource<Notification> completion;

                if (completions.TryDequeue(out completion))
                {
                    completion.SetResult(notification);
                }
                else
                {
                    notifications.Enqueue(notification);
                }
            }
        }
    }
}