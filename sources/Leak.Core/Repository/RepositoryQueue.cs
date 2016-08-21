using System.Collections.Concurrent;

namespace Leak.Core.Repository
{
    public class RepositoryQueue
    {
        private readonly ConcurrentQueue<RepositoryTask> items;

        public RepositoryQueue()
        {
            items = new ConcurrentQueue<RepositoryTask>();
        }

        public void Add(RepositoryTask task)
        {
            items.Enqueue(task);
        }

        public void Process(RepositoryContext context)
        {
            RepositoryTask task;

            while (items.TryDequeue(out task))
            {
                task.Execute(context);
            }
        }
    }
}