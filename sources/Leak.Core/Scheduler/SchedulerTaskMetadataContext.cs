using Leak.Core.Common;
using Leak.Core.Core;
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

        public SchedulerCallback Callback { get; set; }

        public LeakPipeline Pipeline { get; set; }
    }
}