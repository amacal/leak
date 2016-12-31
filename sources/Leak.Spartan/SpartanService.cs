using System;
using Leak.Common;
using Leak.Events;
using Leak.Extensions.Metadata;
using Leak.Tasks;

namespace Leak.Spartan
{
    public class SpartanService : IDisposable
    {
        private readonly SpartanContext context;

        public SpartanService(SpartanParameters parameters, SpartanDependencies dependencies, SpartanHooks hooks, SpartanConfiguration configuration)
        {
            context = new SpartanContext
            {
                Parameters = parameters,
                Dependencies = dependencies,
                Configuration = configuration,
                Hooks = hooks,
                Facts = new SpartanFacts(configuration),
            };

            context.Queue = new LeakQueue<SpartanContext>(context);

            context.Dependencies.Metaget.Hooks.OnMetadataDiscovered += data => context.Queue.Add(new SpartanCompleteDiscover(data));
        }

        public FileHash Hash
        {
            get { return context.Parameters.Hash; }
        }

        public SpartanParameters Parameters
        {
            get { return context.Parameters; }
        }

        public SpartanDependencies Dependencies
        {
            get { return context.Dependencies; }
        }

        public SpartanHooks Hooks
        {
            get { return context.Hooks; }
        }

        public SpartanConfiguration Configuration
        {
            get { return context.Configuration; }
        }

        public void Start()
        {
            context.Dependencies.Pipeline.Register(context.Queue);
            context.Queue.Add(new SpartanScheduleNext(context));
        }

        public void Handle(MetadataMeasured data)
        {
            if (context.Facts.IsOngoing(Goal.Discover))
            {
                context.Dependencies.Metaget.HandleMetadataMeasured(data);
            }
        }

        public void Handle(MetadataReceived data)
        {
            if (context.Facts.IsOngoing(Goal.Discover))
            {
                context.Dependencies.Metaget.HandleMetadataReceived(data);
            }
        }

        public void Handle(PeerChanged data)
        {
            if (context.Facts.IsOngoing(Goal.Download))
            {
                context.Facts.Retriever.HandlePeerChanged(data);
            }
        }

        public void Handle(BlockReceived data)
        {
            if (context.Facts.IsOngoing(Goal.Download))
            {
                context.Facts.Retriever.HandleBlockReceived(data);
            }
        }

        public void Handle(MetadataRequested data)
        {
            context.Dependencies.Metashare.Handle(data);
        }

        public void Dispose()
        {
            if (context.Facts.IsOngoing(Goal.Discover))
            {
                context.Dependencies.Metaget.Stop();
            }

            if (context.Facts.IsOngoing(Goal.Verify))
            {
                context.Facts.Repository.Dispose();
                context.Facts.Repository = null;
            }

            if (context.Facts.IsOngoing(Goal.Download))
            {
                context.Facts.Retriever.Dispose();
                context.Facts.Repository = null;
            }
        }
    }
}