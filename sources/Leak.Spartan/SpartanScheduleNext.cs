using Leak.Common;
using Leak.Tasks;

namespace Leak.Spartan
{
    public class SpartanScheduleNext : LeakTask<SpartanContext>
    {
        private readonly SpartanContext context;

        public SpartanScheduleNext(SpartanContext context)
        {
            this.context = context;
        }

        public void Execute(SpartanContext remove)
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
    }
}