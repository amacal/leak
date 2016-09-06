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

        public override void OnFileStarted(FileHash hash)
        {
            context.Callback.OnDownloadStarted(hash);
        }

        public override void OnFileCompleted(FileHash hash)
        {
            context.Callback.OnDownloadCompleted(hash);
        }

        public override void OnPieceVerified(FileHash hash, PieceInfo piece)
        {
            context.Callback.OnPieceVerified(hash, piece);
        }

        public override void OnFileChanged(FileHash hash, BitfieldInfo bitfield)
        {
            context.Callback.OnDownloadChanged(hash, bitfield);
        }
    }
}