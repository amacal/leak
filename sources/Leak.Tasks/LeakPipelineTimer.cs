using System;

namespace Leak.Tasks
{
    public class LeakPipelineTimer
    {
        private readonly TimeSpan period;
        private readonly Action callback;
        private DateTime next;

        public LeakPipelineTimer(TimeSpan period, Action callback)
        {
            this.period = period;
            this.callback = callback;
            this.next = DateTime.Now.Add(period);
        }

        public void Execute(DateTime now)
        {
            if (next < now)
            {
                next = now.Add(period);
                callback.Invoke();
            }
        }
    }
}