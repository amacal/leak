using System;

namespace Leak.Tasks
{
    public static class LeakQueueExtensions
    {
        public static void Add<TContext>(this LeakQueue<TContext> queue, Action callback)
        {
            queue.Add(new LeakTaskInline<TContext>(callback));
        }
    }
}