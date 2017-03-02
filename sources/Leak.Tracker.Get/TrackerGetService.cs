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
            context.UdpService.Start();
        }

        public void Stop()
        {
            context.UdpService.Stop();
            context.Dependencies.Pipeline.Remove(OnTick);
        }

        public void Register(Uri address, FileHash hash)
        {
            context.Queue.Add(() =>
            {
                context.Collection.Add(address, hash);
            });
        }

        private void OnTick()
        {
            context.Queue.Add(new TrackerGetNextTask());
            context.Queue.Add(new TrackerGetUdpTask());
        }

        public void Dispose()
        {
            context.UdpService.Stop();
            context.Dependencies.Pipeline.Remove(OnTick);
        }
    }
}