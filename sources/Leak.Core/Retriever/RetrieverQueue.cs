using System.Collections.Concurrent;

namespace Leak.Core.Retriever
{
    public class RetrieverQueue
    {
        private readonly ConcurrentQueue<RetrieverTask> items;

        public RetrieverQueue()
        {
            items = new ConcurrentQueue<RetrieverTask>();
        }

        public void Add(RetrieverTask task)
        {
            items.Enqueue(task);
        }

        public void Process(RetrieverContext context)
        {
            RetrieverTask task;

            while (items.TryDequeue(out task))
            {
                task.Execute(context);
            }
        }
    }
}