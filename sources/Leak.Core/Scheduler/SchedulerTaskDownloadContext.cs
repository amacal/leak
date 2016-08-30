using Leak.Core.Common;
using Leak.Core.Messages;
using Leak.Core.Metadata;
using Leak.Core.Retriever;

namespace Leak.Core.Scheduler
{
    public class SchedulerTaskDownloadContext
    {
        public Metainfo Metainfo { get; set; }

        public string Destination { get; set; }

        public Bitfield Bitfield { get; set; }

        public RetrieverService Retriever { get; set; }

        public SchedulerQueue Queue { get; set; }

        public SchedulerTask Task { get; set; }

        public SchedulerCallback Callback { get; set; }
    }
}