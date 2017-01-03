using System;
using Leak.Common;
using Leak.Events;
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
                State = new SpartanState(configuration),
            };

            context.Queue = new LeakQueue<SpartanContext>(context);
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

        public void Handle(MetadataDiscovered data)
        {
            context.State.Metainfo = data.Metainfo;

            context.Hooks.CallTaskCompleted(context.Parameters.Hash, Goal.Discover);
            context.State.Complete(Goal.Discover);

            context.Dependencies.Metaget.Stop();
            context.Queue.Add(new SpartanScheduleNext(context));
        }

        public void Handle(DataVerified data)
        {
            context.Hooks.CallTaskCompleted(context.Parameters.Hash, Goal.Verify);
            context.State.Complete(Goal.Verify);

            context.Queue.Add(new SpartanScheduleNext(context));
        }

        public void Handle(DataCompleted data)
        {
            context.Dependencies.Retriever.Start();
        }

        public void Dispose()
        {
            if (context.State.IsOngoing(Goal.Discover))
            {
                context.Dependencies.Metaget.Stop();
                context.Dependencies.Metashare.Stop();
            }
        }
    }
}