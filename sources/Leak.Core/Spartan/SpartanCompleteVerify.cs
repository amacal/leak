using Leak.Core.Core;
using Leak.Core.Events;

namespace Leak.Core.Spartan
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
            context.Facts.Repository.Dispose();
            context.Facts.Repository = null;

            context.Facts.Complete(SpartanTasks.Verify);
            context.Hooks.CallTaskCompleted(context.Glue.Hash, SpartanTasks.Verify);

            context.Hooks.CallDataVerified(data);
            context.Queue.Add(new SpartanScheduleNext());
        }
    }
}