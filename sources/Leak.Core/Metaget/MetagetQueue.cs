using System.Collections.Concurrent;

namespace Leak.Core.Metaget
{
    public class MetagetQueue
    {
        private readonly ConcurrentQueue<MetagetTask> items;

        public MetagetQueue()
        {
            items = new ConcurrentQueue<MetagetTask>();
        }

        public void Add(MetagetTask task)
        {
            items.Enqueue(task);
        }

        public void Process(MetagetContext context)
        {
            MetagetTask task;

            while (items.TryDequeue(out task))
            {
                task.Execute(context);
            }
        }
    }
}