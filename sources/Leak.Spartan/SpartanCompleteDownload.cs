using Leak.Common;
using Leak.Events;
using Leak.Tasks;

namespace Leak.Spartan
{
    public class SpartanCompleteDownload : LeakTask<SpartanContext>
    {
        private readonly DataCompleted data;

        public SpartanCompleteDownload(DataCompleted data)
        {
            this.data = data;
        }

        public void Execute(SpartanContext context)
        {
            context.Facts.Retriever.Dispose();
            context.Facts.Retriever = null;

            context.Facts.Complete(Goal.Download);
            context.Hooks.CallTaskCompleted(context.Glue.Hash, Goal.Download);

            context.Hooks.CallDataCompleted(data);
        }
    }
}