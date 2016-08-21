using System;

namespace Leak.Core.Scheduler
{
    public class SchedulerService
    {
        private readonly SchedulerContext context;

        public SchedulerService(Action<SchedulerConfiguration> configurer)
        {
            context = new SchedulerContext(configurer);
        }

        public void Handle(Action<SchedulerTaskCallback> callback)
        {
            context.Queue.Notify(callback);
        }

        public void Metadata(Action<SchedulerTaskMetadataContext> configurer)
        {
            context.Queue.Register(new SchedulerTaskMetadata(configurer));
        }

        public void Initialize(Action<SchedulerTaskInitializeContext> configurer)
        {
            context.Queue.Register(new SchedulerTaskInitialize(configurer));
        }
    }
}