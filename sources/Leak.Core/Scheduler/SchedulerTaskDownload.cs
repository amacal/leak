using Leak.Core.Common;
using Leak.Core.Retriever;
using System;
using System.IO;

namespace Leak.Core.Scheduler
{
    public class SchedulerTaskDownload : SchedulerTask
    {
        private readonly SchedulerTaskDownloadContext inside;

        public SchedulerTaskDownload(Action<SchedulerTaskDownloadContext> configurer)
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
            inside.Retriever = new RetrieverService(with =>
            {
                with.Metainfo = inside.Metainfo;
                with.Bitfield = inside.Bitfield;
                with.Strategy = context.Strategy;
                with.Destination = Path.Combine(inside.Destination, $"{inside.Metainfo.Hash}");
                with.Collector = context.Collector.CreateView(inside.Metainfo.Hash);
                with.Callback = new SchedulerTaskDownloadRetrieverCallback(inside);
            });

            inside.Queue = context.Queue;
            inside.Callback = context.Callback;

            inside.Retriever.Start(context.Pipeline);

            return new SchedulerTaskDownloadTaskCallback(inside);
        }
    }
}