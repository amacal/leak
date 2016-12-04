using Leak.Core.Core;
using Leak.Core.Events;

namespace Leak.Core.Spartan
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

            context.Facts.Complete(SpartanTasks.Download);
            context.Hooks.CallTaskCompleted(context.Glue.Hash, SpartanTasks.Download);

            context.Hooks.CallDataCompleted(data);
        }
    }
}