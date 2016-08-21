using Leak.Core.Common;
using Leak.Core.Retriever;

namespace Leak.Core.Scheduler
{
    public class SchedulerTaskDownloadRetrieverCallback : RetrieverCallbackBase
    {
        private readonly SchedulerTaskDownloadContext context;

        public SchedulerTaskDownloadRetrieverCallback(SchedulerTaskDownloadContext context)
        {
            this.context = context;
        }

        public override void OnCompleted(FileHash hash)
        {
            context.Callback.OnCompleted(hash);
        }

        public override void OnPieceVerified(FileHash hash, RetrieverPiece piece)
        {
            context.Callback.OnVerified(hash, piece);
        }
    }
}