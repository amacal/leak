using Leak.Core.Common;
using Leak.Core.Messages;
using Leak.Core.Retriever;
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
            context.Callback.OnFileInitialized(hash, new PeerClientMetainfo(bitfield));
        }

        public override void OnPieceVerified(FileHash hash, RetrieverPiece piece)
        {
            context.Callback.OnPieceVerified(hash, new PeerClientPieceVerification(piece));
        }

        public override void OnDownloadStarted(FileHash hash)
        {
            context.Callback.OnFileStarted(hash);
        }

        public override void OnDownloadCompleted(FileHash hash)
        {
            context.Callback.OnFileCompleted(hash);
        }
    }
}