using System;
using Leak.Tasks;

namespace Leak.Core.Telegraph
{
    public class TelegraphTaskSchedule : LeakTask<TelegraphContext>
    {
        private readonly DateTime now;

        public TelegraphTaskSchedule()
        {
            now = DateTime.Now;
        }

        public void Execute(TelegraphContext context)
        {
            lock (context.Synchronized)
            {
                foreach (TelegraphEntry entry in context.Collection.Expired(now))
                {
                    context.Queue.Add(new TelegraphTaskAnnounce(entry));
                }
            }
        }
    }
}