using Leak.Core.Common;
using Leak.Core.Retriever;

namespace Leak.Core.Scheduler
{
    public interface SchedulerCallback
    {
        void OnCompleted(FileHash hash);

        void OnVerified(FileHash hash, RetrieverPiece piece);
    }
}