using System;
using Leak.Common;
using Leak.Events;
using Leak.Extensions.Metadata;
using Leak.Files;
using Leak.Glue;
using Leak.Tasks;

namespace Leak.Spartan
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
            context.Queue.Add(new SpartanScheduleNext(context));
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

        public void HandlePeerChanged(PeerChanged data)
        {
            if (context.Facts.IsOngoing(SpartanTasks.Download))
            {
                context.Facts.Retriever.HandlePeerChanged(data);
            }
        }

        public void HandleBlockReceived(BlockReceived data)
        {
            if (context.Facts.IsOngoing(SpartanTasks.Download))
            {
                context.Facts.Retriever.HandleBlockReceived(data);
            }
        }

        public void Dispose()
        {
            if (context.Facts.IsOngoing(SpartanTasks.Discover))
            {
                context.Facts.MetaGet.Stop();
                context.Facts.MetaGet = null;
            }

            if (context.Facts.IsOngoing(SpartanTasks.Verify))
            {
                context.Facts.Repository.Dispose();
                context.Facts.Repository = null;
            }

            if (context.Facts.IsOngoing(SpartanTasks.Download))
            {
                context.Facts.Retriever.Dispose();
                context.Facts.Repository = null;
            }
        }
    }
}