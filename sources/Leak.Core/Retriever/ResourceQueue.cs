using System.Collections.Concurrent;
using System.Threading;

namespace Leak.Core.Retriever
{
    public class ResourceQueue
    {
        private readonly ConcurrentQueue<ResourceQueueItem> items;
        private readonly ManualResetEvent synchronized;

        public ResourceQueue()
        {
            this.items = new ConcurrentQueue<ResourceQueueItem>();
            this.synchronized = new ManualResetEvent(false);
        }

        public void Enqueue(ResourceQueueItem item)
        {
            items.Enqueue(item);
        }

        public void Process(ResourceQueueContext context)
        {
            ResourceQueueItem item;

            while (items.TryDequeue(out item))
            {
                item.Handle(context);
            }
        }
    }
}