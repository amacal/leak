using Leak.Common;
using Leak.Events;
using Leak.Tasks;

namespace Leak.Spartan
{
    public class SpartanCompleteVerify : LeakTask<SpartanContext>
    {
        private readonly DataVerified data;

        public SpartanCompleteVerify(DataVerified data)
        {
            this.data = data;
        }

        public void Execute(SpartanContext context)
        {
            context.Facts.Bitfield = data.Bitfield;

            context.Facts.Repository.Dispose();
            context.Facts.Repository = null;

            context.Facts.Complete(Goal.Verify);
            context.Hooks.CallTaskCompleted(context.Glue.Hash, Goal.Verify);

            context.Hooks.CallDataVerified(data);
            context.Queue.Add(new SpartanScheduleNext(context));
        }
    }
}