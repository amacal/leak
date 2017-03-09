using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Leak.Client.Swarm
{
    public class PeerCollection
    {
        private readonly ConcurrentQueue<TaskCompletionSource<SwarmNotification>> completions;
        private readonly ConcurrentQueue<SwarmNotification> notifications;

        public PeerCollection()
        {
            completions = new ConcurrentQueue<TaskCompletionSource<SwarmNotification>>();
            notifications = new ConcurrentQueue<SwarmNotification>();
        }

        public Task<SwarmNotification> Next()
        {
            lock (this)
            {
                SwarmNotification notification;
                TaskCompletionSource<SwarmNotification> completion;

                if (notifications.TryDequeue(out notification))
                {
                    completion = new TaskCompletionSource<SwarmNotification>();
                    completion.SetResult(notification);

                    return completion.Task;
                }

                completion = new TaskCompletionSource<SwarmNotification>();
                completions.Enqueue(completion);

                return completion.Task;
            }
        }

        public void Enqueue(SwarmNotification notification)
        {
            lock (this)
            {
                TaskCompletionSource<SwarmNotification> completion;

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