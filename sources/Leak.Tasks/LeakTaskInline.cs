using System;

namespace Leak.Tasks
{
    public class LeakTaskInline<TContext> : LeakTask<TContext>
    {
        private readonly Action callback;

        public LeakTaskInline(Action callback)
        {
            this.callback = callback;
        }

        public void Execute(TContext context)
        {
            callback.Invoke();
        }
    }
}