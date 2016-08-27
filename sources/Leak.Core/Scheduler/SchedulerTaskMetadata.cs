using Leak.Core.Common;
using Leak.Core.Metaget;
using System;
using System.IO;

namespace Leak.Core.Scheduler
{
    public class SchedulerTaskMetadata : SchedulerTask
    {
        private readonly SchedulerTaskMetadataContext inside;

        public SchedulerTaskMetadata(Action<SchedulerTaskMetadataContext> configurer)
        {
            inside = configurer.Configure(with =>
            {
                with.Task = this;
            });
        }

        public FileHash Hash
        {
            get { return inside.Hash; }
        }

        public SchedulerTaskCallback Start(SchedulerContext context)
        {
            inside.Metaget = new MetagetService(with =>
            {
                with.Hash = inside.Hash;
                with.Destination = Path.Combine(inside.Destination, $"{inside.Hash}.metainfo");
                with.Collector = context.Collector.CreateView(inside.Hash);
                with.Callback = new SchedulerTaskMetadataMetagetCallback(inside);
            });

            inside.Queue = context.Queue;
            inside.Callback = context.Callback;

            inside.Metaget.Start();

            return new SchedulerTaskMetadataTaskCallback(inside);
        }
    }
}