using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Leak.Client.Peer
{
    public class PeerCollection
    {
        private readonly ConcurrentQueue<TaskCompletionSource<PeerNotification>> completions;
        private readonly ConcurrentQueue<PeerNotification> notifications;

        public PeerCollection()
        {
            completions = new ConcurrentQueue<TaskCompletionSource<PeerNotification>>();
            notifications = new ConcurrentQueue<PeerNotification>();
        }

        public Task<PeerNotification> Next()
        {
            lock (this)
            {
                PeerNotification notification;
                TaskCompletionSource<PeerNotification> completion;

                if (notifications.TryDequeue(out notification))
                {
                    completion = new TaskCompletionSource<PeerNotification>();
                    completion.SetResult(notification);

                    return completion.Task;
                }

                completion = new TaskCompletionSource<PeerNotification>();
                completions.Enqueue(completion);

                return completion.Task;
            }
        }

        public void Enqueue(PeerNotification notification)
        {
            lock (this)
            {
                TaskCompletionSource<PeerNotification> completion;

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