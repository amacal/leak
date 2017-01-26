using Leak.Common;
using Leak.Events;
using Leak.Tasks;
using System;

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
            context.Queue.Add(ScheduleNext);
        }

        public void Handle(MetadataDiscovered data)
        {
            context.Queue.Add(() =>
            {
                context.State.Metainfo = data.Metainfo;

                context.Hooks.CallTaskCompleted(context.Parameters.Hash, Goal.Discover);
                context.State.Complete(Goal.Discover);

                context.Dependencies.Metaget.Stop();
                context.Queue.Add(ScheduleNext);
            });
        }

        public void Handle(DataVerified data)
        {
            context.Queue.Add(() =>
            {
                context.Hooks.CallTaskCompleted(context.Parameters.Hash, Goal.Verify);
                context.State.Complete(Goal.Verify);

                context.Queue.Add(ScheduleNext);
            });
        }

        public void Handle(DataCompleted data)
        {
            context.Queue.Add(() =>
            {
                context.Hooks.CallTaskCompleted(context.Parameters.Hash, Goal.Discover);
                context.State.Complete(Goal.Discover);

                context.Dependencies.Retriever.Stop();
            });
        }

        public void Dispose()
        {
            context.Queue.Add(() =>
            {
                StopIfPossible(context.Dependencies.Metaget);
                StopIfPossible(context.Dependencies.Metashare);
                StopIfPossible(context.Dependencies.Retriever);
                StopIfPossible(context.Dependencies.Datashare);
            });
        }

        private void ScheduleNext()
        {
            if (context.State.CanStart(Goal.Discover))
            {
                context.State.Start(Goal.Discover);
                context.Hooks.CallTaskStarted(context.Parameters.Hash, Goal.Discover);

                context.Dependencies.Metaget.Start();
                context.Dependencies.Metashare.Start();

                return;
            }

            if (context.State.CanStart(Goal.Verify))
            {
                context.State.Start(Goal.Verify);
                context.Hooks.CallTaskStarted(context.Parameters.Hash, Goal.Verify);

                context.Dependencies.Repository.Verify(new Bitfield(context.State.Metainfo.Pieces.Length));
            }

            if (context.State.CanStart(Goal.Download))
            {
                context.State.Start(Goal.Download);
                context.Hooks.CallTaskStarted(context.Parameters.Hash, Goal.Download);

                context.Dependencies.Retriever.Start();
                context.Dependencies.Datashare.Start();
            }
        }

        private void StopIfPossible(Component component)
        {
            switch (component.Status)
            {
                case ComponentStatus.Starting:
                case ComponentStatus.Started:
                    component.Stop();
                    break;
            }
        }
    }
}