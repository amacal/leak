using Leak.Core.Metadata;
using Leak.Core.Repository;

namespace Leak.Core.Scheduler
{
    public class SchedulerTaskInitializeContext
    {
        public Metainfo Metainfo { get; set; }

        public string Destination { get; set; }

        public RepositoryService Repository { get; set; }

        public SchedulerQueue Queue { get; set; }

        public SchedulerTask Task { get; set; }

        public SchedulerCallback Callback { get; set; }
    }
}