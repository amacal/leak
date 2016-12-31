using Leak.Common;
using Leak.Events;
using Leak.Tasks;

namespace Leak.Spartan
{
    public class SpartanCompleteDiscover : LeakTask<SpartanContext>
    {
        private readonly MetadataDiscovered data;

        public SpartanCompleteDiscover(MetadataDiscovered data)
        {
            this.data = data;
        }

        public void Execute(SpartanContext context)
        {
            context.Dependencies.Metaget.Stop();

            context.Facts.Complete(Goal.Discover);
            context.Hooks.CallTaskCompleted(context.Parameters.Hash, Goal.Discover);

            context.Facts.Metainfo = data.Metainfo;
            context.Queue.Add(new SpartanScheduleNext(context));
        }
    }
}