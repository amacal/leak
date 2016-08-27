using Leak.Core.Common;
using Leak.Core.Messages;
using Leak.Core.Retriever;

namespace Leak.Core.Scheduler
{
    public abstract class SchedulerCallbackBase : SchedulerCallback
    {
        public virtual void OnMetadataCompleted(FileHash hash)
        {
        }

        public virtual void OnResourceInitialized(FileHash hash, Bitfield bitfield)
        {
        }

        public virtual void OnPieceVerified(FileHash hash, RetrieverPiece piece)
        {
        }

        public virtual void OnDownloadStarted(FileHash hash)
        {
        }

        public virtual void OnDownloadCompleted(FileHash hash)
        {
        }
    }
}