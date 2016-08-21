using Leak.Core.Common;
using Leak.Core.Metaget;

namespace Leak.Core.Scheduler
{
    public class SchedulerTaskMetadataContext
    {
        public FileHash Hash { get; set; }

        public string Destination { get; set; }

        public MetagetService Metaget { get; set; }

        public SchedulerQueue Queue { get; set; }

        public SchedulerTask Task { get; set; }
    }
}