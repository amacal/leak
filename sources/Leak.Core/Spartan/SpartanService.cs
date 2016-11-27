using Leak.Core.Core;
using Leak.Core.Glue;
using Leak.Core.Glue.Extensions.Metadata;
using Leak.Files;
using System;

namespace Leak.Core.Spartan
{
    public class SpartanService : IDisposable
    {
        private readonly SpartanContext context;

        public SpartanService(LeakPipeline pipeline, string destination, GlueService glue, FileFactory files, SpartanHooks hooks, SpartanConfiguration configuration)
        {
            context = new SpartanContext
            {
                Destination = destination,
                Pipeline = pipeline,
                Glue = glue,
                Files = files,
                Hooks = hooks,
                Facts = new SpartanFacts(configuration),
            };

            context.Queue = new LeakQueue<SpartanContext>(context);
        }

        public void Start()
        {
            context.Pipeline.Register(context.Queue);
            context.Queue.Add(new SpartanScheduleNext());
        }

        public void HandleMetadataMeasured(MetadataMeasured data)
        {
            if (context.Facts.IsOngoing(SpartanTasks.Discover))
            {
                context.Facts.MetaGet.HandleMetadataMeasured(data);
            }
        }

        public void HandleMetadataReceived(MetadataReceived data)
        {
            if (context.Facts.IsOngoing(SpartanTasks.Discover))
            {
                context.Facts.MetaGet.HandleMetadataReceived(data);
            }
        }

        public void Dispose()
        {
            if (context.Facts.IsOngoing(SpartanTasks.Discover))
            {
                context.Facts.MetaGet.Stop(context.Pipeline);
                context.Facts.MetaGet = null;
            }

            if (context.Facts.IsOngoing(SpartanTasks.Verify))
            {
                context.Facts.Repository.Dispose();
                context.Facts.Repository = null;
            }
        }
    }
}