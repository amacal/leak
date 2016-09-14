using Leak.Core.Common;
using Leak.Core.Repository;

namespace Leak.Core.Scheduler
{
    public class SchedulerTaskInitializeRepositoryCallback : RepositoryCallbackBase
    {
        private readonly SchedulerTaskInitializeContext context;

        public SchedulerTaskInitializeRepositoryCallback(SchedulerTaskInitializeContext context)
        {
            this.context = context;
        }

        public override void OnAllocated(FileHash hash, RepositoryAllocation allocation)
        {
            context.Repository.Verify(allocation.ToBitfield());
        }

        public override void OnVerified(FileHash hash, Bitfield bitfield)
        {
            SchedulerTaskDownload download = new SchedulerTaskDownload(with =>
            {
                with.Bitfield = bitfield;
                with.Metainfo = context.Metainfo;
                with.Destination = context.Destination;
            });

            context.Callback.OnResourceInitialized(hash, bitfield);

            context.Queue.Complete(context.Task);
            context.Queue.Register(download);
        }
    }
}