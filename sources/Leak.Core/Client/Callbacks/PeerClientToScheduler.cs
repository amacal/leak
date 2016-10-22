using Leak.Core.Client.Events;
using Leak.Core.Common;
using Leak.Core.Scheduler;

namespace Leak.Core.Client.Callbacks
{
    public class PeerClientToScheduler : SchedulerCallbackBase
    {
        private readonly PeerClientContext context;

        public PeerClientToScheduler(PeerClientContext context)
        {
            this.context = context;
        }

        public override void OnMetadataCompleted(FileHash hash)
        {
            context.Callback.OnFileDiscovered(hash);
        }

        public override void OnResourceInitialized(FileHash hash, Bitfield bitfield)
        {
            context.Callback.OnFileInitialized(hash, new FileInitializedEvent(bitfield));
        }

        public override void OnPieceVerified(FileHash hash, PieceInfo piece)
        {
            context.Callback.OnPieceVerified(hash, new PieceVerifiedEvent(hash, piece));
        }

        public override void OnPieceRejected(FileHash hash, PieceInfo piece)
        {
            context.Callback.OnPieceRejected(hash, new PieceRejectedEvent(hash, piece));
        }

        public override void OnDownloadStarted(FileHash hash)
        {
            context.Callback.OnFileStarted(hash);
        }

        public override void OnDownloadChanged(FileHash hash, BitfieldInfo bitfield)
        {
            context.Callback.OnFileChanged(hash, bitfield);
        }

        public override void OnDownloadCompleted(FileHash hash)
        {
            context.Callback.OnFileCompleted(hash);
        }
    }
}