using System;
using System.Threading;

namespace Leak.Core.Core
{
    public class LeakTimer
    {
        private readonly TimeSpan period;
        private Timer timer;

        public LeakTimer(TimeSpan period)
        {
            this.period = period;
        }

        public void Start(Action callback)
        {
            TimerCallback onTick = state =>
            {
                Disable();

                try
                {
                    if (timer != null)
                    {
                        callback.Invoke();
                    }
                }
                finally
                {
                    Enable();
                }
            };

            timer = new Timer(onTick);
            Enable();
        }

        public void Stop()
        {
            Disable();
        }

        public void Dispose()
        {
            timer?.Dispose();
            timer = null;
        }

        private void Enable()
        {
            timer?.Change(period, period);
        }

        private void Disable()
        {
            timer?.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }
}