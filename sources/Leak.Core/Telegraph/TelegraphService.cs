using Leak.Core.Tracker;
using Leak.Tasks;
using System;

namespace Leak.Core.Telegraph
{
    public class TelegraphService
    {
        private readonly TelegraphContext context;

        public TelegraphService(Action<TelegraphConfiguration> configurer)
        {
            context = new TelegraphContext(configurer);
        }

        public void Start(LeakPipeline pipeline)
        {
            pipeline.Register(context.Queue);
            pipeline.Register(TimeSpan.FromSeconds(5), OnTick);
        }

        private void OnTick()
        {
            context.Queue.Add(new TelegraphTaskSchedule());
        }

        public void Register(string tracker, Action<TrackerRequest> configurer)
        {
            lock (context.Synchronized)
            {
                TrackerRequest request = configurer.Configure(with =>
                {
                    with.Port = context.Configuration.Port;
                    with.Peer = context.Configuration.Peer;

                    configurer.Invoke(with);
                });

                TelegraphEntry entry = context.Collection.GetOrCreate(request.Hash, tracker);

                entry.Request = request;
                entry.Next = DateTime.Now;
            }
        }
    }
}