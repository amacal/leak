using System;
using System.Threading;

namespace Leak.Core.Metaget
{
    public class MetagetTimer
    {
        private readonly TimeSpan period;

        public MetagetTimer(TimeSpan period)
        {
            this.period = period;
        }

        public void Start(Action callback)
        {
            Timer timer = null;
            TimerCallback onTick = state =>
            {
                Disable(timer);

                try
                {
                    callback.Invoke();
                }
                finally
                {
                    Enable(timer);
                }
            };

            Enable(timer = new Timer(onTick));
        }

        private void Enable(Timer timer)
        {
            timer.Change(period, period);
        }

        private void Disable(Timer timer)
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }
}