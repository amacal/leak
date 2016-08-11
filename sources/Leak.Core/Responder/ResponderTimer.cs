using System;
using System.Threading;

namespace Leak.Core.Responder
{
    public class ResponderTimer
    {
        private readonly ResponderContext context;
        private readonly Timer inner;

        public ResponderTimer(ResponderContext context)
        {
            this.context = context;
            this.inner = new Timer(OnTick, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        public void Start()
        {
            inner.Change(TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60));
        }

        private void OnTick(object state)
        {
            inner.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

            try
            {
                ResponderEntry[] entries;
                DateTime now = DateTime.Now;

                lock (context.Synchronized)
                {
                    entries = context.Collection.Find(now);
                }

                foreach (ResponderEntry entry in entries)
                {
                    entry.NextKeepAlive = DateTime.Now.AddMinutes(1);
                    entry.Channel.SendKeepAlive();
                }
            }
            finally
            {
                inner.Change(TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60));
            }
        }
    }
}