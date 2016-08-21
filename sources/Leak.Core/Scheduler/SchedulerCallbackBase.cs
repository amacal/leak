using Leak.Core.Common;
using Leak.Core.Retriever;

namespace Leak.Core.Scheduler
{
    public abstract class SchedulerCallbackBase : SchedulerCallback
    {
        public virtual void OnCompleted(FileHash hash)
        {
        }

        public virtual void OnVerified(FileHash hash, RetrieverPiece piece)
        {
        }
    }
}