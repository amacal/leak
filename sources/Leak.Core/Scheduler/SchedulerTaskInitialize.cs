using Leak.Core.Common;
using Leak.Core.Repository;
using System;
using System.IO;

namespace Leak.Core.Scheduler
{
    public class SchedulerTaskInitialize : SchedulerTask
    {
        private readonly SchedulerTaskInitializeContext inside;

        public SchedulerTaskInitialize(Action<SchedulerTaskInitializeContext> configurer)
        {
            inside = configurer.Configure(with =>
            {
                with.Task = this;
            });
        }

        public FileHash Hash
        {
            get { return inside.Metainfo.Hash; }
        }

        public SchedulerTaskCallback Start(SchedulerContext context)
        {
            inside.Repository = new RepositoryService(with =>
            {
                with.Metainfo = inside.Metainfo;
                with.Destination = Path.Combine(inside.Destination, $"{inside.Metainfo.Hash}");
                with.Callback = new SchedulerTaskInitializeRepositoryCallback(inside);
            });

            inside.Queue = context.Queue;
            inside.Callback = context.Callback;

            inside.Repository.Allocate();
            inside.Repository.Verify();

            return new SchedulerTaskCallbackNothing();
        }
    }
}