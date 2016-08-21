using Leak.Core.Common;
using Leak.Core.Metadata;
using Leak.Core.Metaget;

namespace Leak.Core.Scheduler
{
    public class SchedulerTaskMetadataMetagetCallback : MetagetCallbackBase
    {
        private readonly SchedulerTaskMetadataContext context;

        public SchedulerTaskMetadataMetagetCallback(SchedulerTaskMetadataContext context)
        {
            this.context = context;
        }

        public override void OnMetadataCompleted(FileHash hash, Metainfo metainfo)
        {
            SchedulerTaskInitialize initialize = new SchedulerTaskInitialize(with =>
            {
                with.Metainfo = metainfo;
                with.Destination = context.Destination;
            });

            context.Metaget.Stop();
            context.Metaget.Dispose();

            context.Queue.Complete(context.Task);
            context.Queue.Register(initialize);
        }
    }
}