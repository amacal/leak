using Leak.Tasks;
using System;
using Leak.Common;

namespace Leak.Tracker.Get
{
    public class TrackerGetService : IDisposable
    {
        private readonly TrackerGetContext context;

        public TrackerGetService(TrackerGetParameters parameters, TrackerGetDependencies dependencies, TrackerGetConfiguration configuration, TrackerGetHooks hooks)
        {
            context = new TrackerGetContext(parameters, dependencies, configuration, hooks);
        }

        public TrackerGetHooks Hooks
        {
            get { return context.Hooks; }
        }

        public TrackerGetParameters Parameters
        {
            get { return context.Parameters; }
        }

        public TrackerGetDependencies Dependencies
        {
            get { return context.Dependencies; }
        }

        public TrackerGetConfiguration Configuration
        {
            get { return context.Configuration; }
        }

        public void Start()
        {
            context.Dependencies.Pipeline.Register(context.Queue);
            context.Dependencies.Pipeline.Register(1000, OnTick);

            context.Udp.Start();
            context.Http.Start();
        }

        public void Stop()
        {
            context.Queue.Stop();
            context.Dependencies.Pipeline.Remove(OnTick);

            context.Udp.Stop();
            context.Udp.Stop();
        }

        public void Register(TrackerGetRegistrant registrant)
        {
            context.Queue.Add(() =>
            {
                context.Collection.Add(registrant);
            });
        }

        public void Announce(FileHash hash)
        {
            context.Queue.Add(new TrackerGetAnnounceTask(hash));
        }

        private void OnTick()
        {
            context.Queue.Add(new TrackerGetNextTask());
            context.Queue.Add(new TrackerGetHttpTask());
            context.Queue.Add(new TrackerGetUdpTask());
        }

        public void Dispose()
        {
            Stop();
        }
    }
}