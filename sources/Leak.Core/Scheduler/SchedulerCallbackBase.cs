using Leak.Core.Common;

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

        public virtual void OnPieceVerified(FileHash hash, PieceInfo piece)
        {
        }

        public virtual void OnDownloadStarted(FileHash hash)
        {
        }

        public virtual void OnDownloadChanged(FileHash hash, BitfieldInfo bitfield)
        {
        }

        public virtual void OnDownloadCompleted(FileHash hash)
        {
        }
    }
}